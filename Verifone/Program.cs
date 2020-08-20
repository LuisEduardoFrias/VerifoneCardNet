namespace Verifone
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    class Program
    {
        private static string _IPMaquina, _IPVerifone, _Datos, _Entero, _path, _Transaccion, _FechaHora, _TipoVerifone;
        private static int _PuertoMaquina, _PuertoVerifone,_Monto, _Itbis, _Tikes, _OtroImpuesto, _Contador, _Conteotransacciones = 0, _Diferido;
        private static bool _procesar, _Exception;
        private static FileInfo _NombreArchivo;

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

                            File.Move(_path + "\\entradas\\" + _NombreArchivo.Name, _path + "\\PeticionesProcesadas\\" + _NombreArchivo.Name);

                            File.Delete(_path + "\\entradas\\" + _NombreArchivo.Name);

                        }
                        catch (Exception ex) { }

                    } while (_procesar == false);

                    char[] b = new char[_Datos.Length];

                    using (StringReader sr = new StringReader(_Datos))
                    {
                        sr.Read(b, 0, _Datos.Length);

                        foreach (var caracteres in b)
                        {
                            if (caracteres.Equals(';'))
                            {
                                _Contador++;
                                _Entero = string.Empty;
                            }
                            else
                            {
                                if(!caracteres.Equals('\0') && !caracteres.Equals('\n'))
                                {
                                    switch (_Contador)
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
                catch (Exception ex)
                {
                    execcion(ex.Message);

                    _Exception = true;
                }

                if(verificacion())
                {
                    if (_TipoVerifone == "CARDNET")
                    {
                        _Conteotransacciones++;

                        Coneccion con = new Coneccion(_IPMaquina, _PuertoMaquina, _IPVerifone, _PuertoVerifone, _path, _Conteotransacciones, _NombreArchivo.Name.Substring(0, Convert.ToInt32(_NombreArchivo.Name.Length) - 4));

                        if (_Transaccion == "CU00")
                        {
                            if (!con.EnviarRecibirPOS(_Transaccion + "" + _Monto + "" + _Itbis + "" + _OtroImpuesto + "" + _Tikes + "" + _Diferido))
                                _Conteotransacciones--;
                        }
                        else if (_Transaccion.Equals("CN00") || _Transaccion.Equals("CN01"))
                        {
                            if (!con.EnviarRecibirPOS(_Transaccion + "" + _Monto + "" + _Itbis + "" + _OtroImpuesto + "" + _Tikes))
                                _Conteotransacciones--;
                        }

                        _Exception = false;
                    }
                    else if (_TipoVerifone == "VISANET")
                    {

                    }
                }
                else
                {
                    execcion();
                }
                    

               

            } while (true);
           
        }

        private static bool verificacion()
        {
            try
            {
                string HoraFecha = DateTime.Now.ToString("dd,MM,yyyy") + "-" + DateTime.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                string dataFH1 = HoraFecha.Substring(0, 10);
                string dataFH2 = _FechaHora.Substring(0, 10);

                if (dataFH1 == dataFH2)
                {
                    dataFH1 = HoraFecha.Substring(11);
                    dataFH2 = _FechaHora.Substring(11);

                    if (dataFH1.Substring(0, 2) == dataFH2.Substring(0, 2))
                    {
                        if (int.Parse(dataFH1.Substring(3, 2)) <= (int.Parse(dataFH2.Substring(3, 2)) + 2))
                        {
                            if (!_Exception)
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
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void execcion(string ex)
        {
            Console.WriteLine("Error1 :" + ex);

            var ruta = _path + "\\respuesta\\" + (_NombreArchivo.Name.Substring(0, Convert.ToInt32(_NombreArchivo.Name.Length) - 4)) + ".txt";

            using (StreamWriter Answer = File.CreateText(ruta))
            {
                Answer.WriteLine("Error:");
                Answer.WriteLine(ex);
            }

            File.Move(ruta, ruta.Substring(0, (Convert.ToInt32(ruta.Length)) - 4) + ".rsp");
        }

        private static void execcion()
        {
            var ruta = _path + "\\respuesta\\" + (_NombreArchivo.Name.Substring(0, Convert.ToInt32(_NombreArchivo.Name.Length) - 4)) + ".txt";

            using (StreamWriter Answer = File.CreateText(ruta))
            {
                Answer.WriteLine("Error:");
            }

            File.Move(ruta, ruta.Substring(0, (Convert.ToInt32(ruta.Length)) - 4) + ".rsp");
        }
    }
}