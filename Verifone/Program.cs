namespace Verifone
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using varifoneVisaNet;

    class Program
    {
        private static string _IPMaquina, _IPVerifone, _Datos, _Entero, _path, _Transaccion, _FechaHora, _TipoVerifone;
        private static int _PuertoMaquina, _PuertoVerifone,_Monto, _Itbis, _Tikes, _OtroImpuesto, _Contador, _Conteotransacciones = 0, _Diferido;
        private static bool _procesar, _Exception;
        private static FileInfo _NombreArchivo;

        private static string _Port;
        private static int _Speed, _DataBit;
        private static Parity _Parity;
        private static StopBits _StropBits;
        private static Handshake _Handshake;


        static void Main(string[] args)
        {
            _path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            do
            {

                #region initializing variables

                _IPMaquina = string.Empty;
                _IPVerifone = string.Empty;
                _PuertoMaquina = 0;
                _PuertoVerifone = 0;
                _FechaHora = string.Empty;
                _TipoVerifone = string.Empty;

                _Transaccion = string.Empty;
                _Datos = string.Empty;
                _Entero = string.Empty;

                _procesar = false;

                _Diferido = 0;
                _Monto = 0;
                _Itbis = 0;
                _Tikes = 0;
                _OtroImpuesto = 0;
                _Contador = 1;

                #endregion

                try
                {
                    do
                    {
                        try
                        {
                            DirectoryInfo Archivo = new DirectoryInfo(_path + "\\entradas\\");

                            _NombreArchivo = Archivo.GetFiles("*.txt")[0];

                            _Datos = File.ReadAllText(_path + "\\entradas\\" + _NombreArchivo.Name);

                            _procesar = true;

                            File.Move(_path + "\\entradas\\" + _NombreArchivo.Name,_path + "\\PeticionesProcesadas\\" + _NombreArchivo.Name);

                            File.Delete(_path + "\\entradas\\" + _NombreArchivo.Name);

                        }
                        catch (Exception ex) { }

                    } while (_procesar == false);

                    char[] b = new char[_Datos.Length];

                    TranformarEntrada(b);

                }
                catch (Exception ex)
                {
                    Execcion(ex.Message);

                    _Exception = true;
                }

                if(VerificacionFechaHora())
                {
                    if(_TipoVerifone == "CARDNET")
                    {
                        _Conteotransacciones++;

                        ConectionCardNet con = new ConectionCardNet(_IPMaquina,_PuertoMaquina,_IPVerifone,_PuertoVerifone,_path,_Conteotransacciones,_NombreArchivo.Name.Substring(0,Convert.ToInt32(_NombreArchivo.Name.Length) - 4));

                        if(_Transaccion == "CU00")
                        {
                            if(!con.EnviarRecibirPOS(_Transaccion + "" + _Monto + "" + _Itbis + "" + _OtroImpuesto + "" + _Tikes + "" + _Diferido))
                                _Conteotransacciones--;
                        }
                        else if(_Transaccion.Equals("CN00") || _Transaccion.Equals("CN01"))
                        {
                            if(!con.EnviarRecibirPOS(_Transaccion + "" + _Monto + "" + _Itbis + "" + _OtroImpuesto + "" + _Tikes))
                                _Conteotransacciones--;
                        }

                        _Exception = false;
                    }
                    else if(_TipoVerifone == "VISANET")
                    {

                        _Port = System.Configuration.ConfigurationManager.AppSettings.Get("Port");

                        _Speed = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("Speed"));

                        _Parity = (Parity)Enum.Parse(typeof(Parity),
                            System.Configuration.ConfigurationManager.AppSettings.Get("Parity"),
                            true);

                        _DataBit = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("DataBit"));

                        _StropBits = (StopBits)Enum.Parse(typeof(StopBits),
                            System.Configuration.ConfigurationManager.AppSettings.Get("StropBits"),
                            true);

                        _Handshake = (Handshake)Enum.Parse(typeof(Handshake),
                            System.Configuration.ConfigurationManager.AppSettings.Get("Handshake"),
                            true);

                        ConectionVisaNet comunicacionPort = ConectionVisaNet.GetIntances("COM3",9600,Parity.None,8,StopBits.One);

                        comunicacionPort.OpenComunication(_Handshake);
                        var respuesta = comunicacionPort.SendDataAsync(ConfigureTrama(),new TimeSpan(DateTime.Now.Hour,DateTime.Now.Second,DateTime.Now.Millisecond));
                        comunicacionPort.CloseComunication();

                        var error = "99";

                        if(respuesta.Substring(0,error.Length) == error || respuesta == "Time is up")
                        {
                            Execcion(respuesta);
                        }
                        else
                        {
                            Reply(respuesta);
                        }

                    }
                }
                else
                {
                    Execcion("El Tiempo Expiro.");
                }
                    
            } while (true);
           
        }

        private static string ConfigureTrama()
        {

            return "\u0002" + _Transaccion + "\u001c" + _Monto + "\u001c" + _Itbis + "\u001c" + _OtroImpuesto + "\u001c" + _Tikes + "\u001c" + _Diferido + "\u0003\u0016";    

        }


        private static bool VerificacionFechaHora()
        {
            try
            {
                int index1 = 0;
                string _Fecha1 = string.Empty;
                string Hora1 = string.Empty;

                for(int i = 0; i < _FechaHora.Length; i++)
                {

                    var caracter = _FechaHora.Substring(index1,1);

                    if(caracter == ",")
                        _Fecha1 += "/";
                    else if(caracter == "-")
                        _Fecha1 += " ";
                    else if(index1 <= 9)
                        _Fecha1 += caracter;
                    else if(index1 >= 10)
                        Hora1 += caracter;

                    index1++;
                }

                TimeSpan _Hora1 = TimeSpan.Parse(Hora1);

                string _Fecha2 = DateTime.Now.ToString("dd/MM/yyyy");
                TimeSpan _Hora2 = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture));

                TimeSpan residuo = _Hora2.Subtract(_Hora1);

                if(_Fecha1.Trim() == _Fecha2.Trim())
                {
                    if(residuo.Minutes <= 2)
                    {
                        if(!_Exception)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch(Exception ex)
            {
                return false;
            }
        }


        private static void Execcion(string ex)
        {
            Console.WriteLine("\nError :" + ex);

            var ruta = _path + "\\respuesta\\" + (_NombreArchivo.Name.Substring(0, Convert.ToInt32(_NombreArchivo.Name.Length) - 4)) + ".txt";

            using (StreamWriter Answer = File.CreateText(ruta))
            {
                Answer.Write("Error:");
                Answer.Write(ex);
            }

            File.Move(ruta, ruta.Substring(0, (Convert.ToInt32(ruta.Length)) - 4) + ".rsp");
        }


        private static void Reply(string repuesta)
        {
            try
            {
                var ruta = _path + "\\respuesta\\" + (_NombreArchivo.Name.Substring(0,Convert.ToInt32(_NombreArchivo.Name.Length) - 4)) + ".txt";

                using(StreamWriter Answer = File.CreateText(ruta))
                {
                    Answer.WriteLine(repuesta);
                }

                File.Move(ruta,ruta.Substring(0,(Convert.ToInt32(ruta.Length)) - 4) + ".rsp");
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
            }
        }


        private static void TranformarEntrada(char[] b)
        {
            using(StringReader sr = new StringReader(_Datos))
            {
                sr.Read(b,0,_Datos.Length);

                foreach(var caracteres in b)
                {
                    if(caracteres.Equals(';'))
                    {
                        _Contador++;
                        _Entero = string.Empty;
                    }
                    else
                    {
                        if(!caracteres.Equals('\0') && !caracteres.Equals('\n'))
                        {
                            switch(_Contador)
                            {
                                case 1:
                                    {
                                        _Transaccion += Convert.ToString(caracteres).Trim();

                                        break;
                                    }
                                case 2:
                                    {
                                        _IPMaquina += Convert.ToString(caracteres).Trim();

                                        break;
                                    }
                                case 3:
                                    {
                                        _Entero += Convert.ToString(caracteres).Trim();
                                        _PuertoMaquina = Convert.ToInt32(_Entero);

                                        break;
                                    }
                                case 4:
                                    {
                                        _IPVerifone += Convert.ToString(caracteres).Trim();

                                        break;
                                    }
                                case 5:
                                    {
                                        _Entero += Convert.ToString(caracteres).Trim();
                                        _PuertoVerifone = Convert.ToInt32(_Entero);

                                        break;
                                    }
                                case 6:
                                    {
                                        _Entero += Convert.ToString(caracteres).Trim();
                                        _Monto = Convert.ToInt32(_Entero);
                                        break;
                                    }
                                case 7:
                                    {
                                        _Entero += Convert.ToString(caracteres).Trim();
                                        _Itbis = Convert.ToInt32(_Entero);
                                        break;
                                    }
                                case 8:
                                    {
                                        _Entero += Convert.ToString(caracteres).Trim();
                                        _OtroImpuesto += Convert.ToInt32(Convert.ToString(caracteres).Trim());
                                        break;
                                    }
                                case 9:
                                    {
                                        _Entero += Convert.ToString(caracteres).Trim();
                                        _Tikes = Convert.ToInt32(_Entero);
                                        break;
                                    }
                                case 10:
                                    {
                                        _Entero += Convert.ToString(caracteres).Trim();
                                        _Diferido += Convert.ToInt32(Convert.ToString(caracteres).Trim());
                                        break;
                                    }
                                case 11:
                                    {
                                        _FechaHora += Convert.ToString(caracteres).Trim();
                                        break;
                                    }
                                case 12:
                                    {
                                        _TipoVerifone += Convert.ToString(caracteres).Trim();
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        }


    }
}