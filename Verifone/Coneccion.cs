namespace Verifone
{
    using System;
    using System.Text;
    using System.Net.Sockets;
    using System.Net;
    using System.IO;

    public class Coneccion
    {

        #region Propiedades

        private Socket RemoteEndPoint { get; set; }

       // private Socket _Socket { get; set; }

        private EndPoint ConnectRemota { get; set; }

        private EndPoint ConnectLocal { get; set; }

        private int Timeout = 30789;
        byte[] SentAndReceivedBytes = new byte[1024];

        private string _PathRepuesta, _nombreArchivo;
        private StreamWriter _Answer;
        private int _Contador;

        #region Valores
        private string ENQ = ""; // ENQ-> Enquire-> 5 -> \u0005 -> 
        private string ACK = ""; // ACK-> Acknowledge-> 06 \u0006 -> 
        private string EOM = ""; // EM-> End of Medium-> 19 \u0019 -> 
        private string SYN = ""; // SYN-> Synchronous Idle-> 16 \u0016 -> 
        private string EOT = ""; // EOT-> End Of Transmission-> 04 \u0004 -> 
        private string CardConsultation => "CS00";

        DateTime fecha = new DateTime();
        #endregion

        #endregion


        public Coneccion(string ipLocal, int puertoLocal, string ipRemota, int puertoRemoto, string PathRepuesta,int Contador,string nombreArchivo)
        {
            try
            {
                _Contador = Contador;
                _nombreArchivo = nombreArchivo;
                _PathRepuesta = PathRepuesta;
                _Answer = File.CreateText(_PathRepuesta  + "\\" + _nombreArchivo + ".txt");

                ConnectRemota = new IPEndPoint(IPAddress.Parse(ipRemota), puertoRemoto);
                ConnectLocal = new IPEndPoint(IPAddress.Parse(ipLocal), puertoLocal);

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

        }


        //// primer metodo
        public bool EnviarRecibirPOS(string trama)
        {
            try
            {
                if (Enquire().Equals(ACK))
                {
                    if (Synchronous(out SentAndReceivedBytes).Equals(EOM))
                    {
                        if (ECRPOS(trama))
                        {

                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            catch (Exception ex)
            {

                _Answer.WriteLine("Error:");
                _Answer.WriteLine(ex.Message.ToString());

                return false;

            }
            finally
            {
                _Answer.Close();
                

                try
                {
                    var g = _PathRepuesta + "\\" + _nombreArchivo + ".txt";
                    var gg = _PathRepuesta + "\\respuesta\\" + _nombreArchivo + ".rsp";
                    File.Move(g,gg);
                    File.Delete(_PathRepuesta + _nombreArchivo + ".txt");
                }
                catch (Exception ex) { }
            }
        }


        //// segundo metodo
        private string Enquire()
        {
            using (RemoteEndPoint = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                RemoteEndPoint.ReceiveTimeout = Timeout;
                RemoteEndPoint.SendTimeout = Timeout;
                RemoteEndPoint.Connect(ConnectRemota);

                var buffer = Encoding.Default.GetBytes(ENQ);

                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("");
                Console.WriteLine("Trnsaccion {0} del dia.", _Contador);
                Console.WriteLine();
                Console.WriteLine("Enquire -> Envio {0}", ENQ);

                RemoteEndPoint.Send(buffer);
                RemoteEndPoint.Receive(buffer);

                var recibedData = Encoding.ASCII.GetString(buffer); // Se espera: ACK =  = \u0006

                Console.WriteLine("Enquire <- Recibi: {0}", recibedData.ToString());

                if (recibedData.Equals(ACK))
                {
                    return ACK;
                }
                else
                {
                    return null;
                }
            }
        }


        //// tercero metodo
        private string Synchronous(out byte[] buffer)
        {
            using (RemoteEndPoint = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                RemoteEndPoint.ReceiveTimeout = Timeout;
                RemoteEndPoint.SendTimeout = Timeout;
                RemoteEndPoint.Connect(ConnectRemota);


                buffer = Encoding.Default.GetBytes(SYN);

                Console.WriteLine("SYN -> Envio {0}", SYN);

                RemoteEndPoint.Send(buffer);
                RemoteEndPoint.Receive(buffer);


                var recibedData = Encoding.ASCII.GetString(buffer);

                Console.WriteLine("SYN <- Recibi: {0} ", recibedData.ToString());

                if (recibedData.Equals(EOM))
                    return EOM;
                else
                    return null;
            }
        }


        //// cuarto metodo
        private bool ECRPOS(string trama)
        {
            string data = null;
            char dataChar;
            byte[] bytes = new Byte[1024];

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ConnectLocal.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ConnectLocal);
                listener.Listen(5);
                listener.ReceiveTimeout = Timeout;
                listener.SendTimeout = Timeout;

                Console.WriteLine("connection...");

                Socket handler = listener.Accept();

                data = null;


                while (true)
                {
                    handler.LingerState = new LingerOption(true, 10);

                    handler.Receive(SentAndReceivedBytes);

                    data += Encoding.ASCII.GetString(SentAndReceivedBytes);

                    Console.WriteLine("", data);

                    if (data.Equals(ENQ))
                    {
                        SentAndReceivedBytes = (new ASCIIEncoding()).GetBytes(trama);

                        Console.WriteLine(trama.ToString());

                        handler.Send(SentAndReceivedBytes);

                        var port = ((IPEndPoint)handler.RemoteEndPoint).Port;

                        Console.WriteLine("envia Trama", Encoding.ASCII.GetString(SentAndReceivedBytes));

                        handler.Receive(SentAndReceivedBytes);

                        dataChar = Convert.ToChar(SentAndReceivedBytes[0]);

                        Console.WriteLine("recibo ACK {0}", dataChar);

                        if (dataChar.ToString().Equals(ACK))
                        {
                            do
                            {
                                Console.WriteLine("Muestra el Handler");
                                if (handler.Connected)
                                {
                                    SentAndReceivedBytes = new byte[1024];

                                    handler.Receive(SentAndReceivedBytes);

                                    dataChar = Convert.ToChar(SentAndReceivedBytes[0]);

                                    Console.WriteLine("EOT {0}", dataChar.ToString());

                                    if (!dataChar.ToString().Equals(EOT))
                                    {

                                        data = Encoding.ASCII.GetString(SentAndReceivedBytes).Replace("\0", "");


                                        Console.WriteLine("recibe Trama", data);

                                        SentAndReceivedBytes = (new ASCIIEncoding()).GetBytes(ACK.ToString());

                                        handler.Send(SentAndReceivedBytes);

                                    }
                                }

                            } while (!dataChar.ToString().Equals(EOT));


                            _Answer.WriteLine(data);
                            Console.WriteLine("antes del Break: {0}", data);
                        }

                        handler.Send((new ASCIIEncoding()).GetBytes(ACK));

                        Console.WriteLine("Envia ACK {0}", ACK);

                        data = handler.Receive(SentAndReceivedBytes).ToString();

                        Console.WriteLine("Recibe {0}", data);
                        break;
                    }
                };

                Console.WriteLine("Final del todo : {0}", data);


                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());


                _Answer.WriteLine("Error:");
                _Answer.WriteLine(ex.ToString());


                Acknowledge();

                return false;
            }
            finally { listener.Close(); }

        }


        //// quinto metodo
        private string Acknowledge()
        {
            using (RemoteEndPoint = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                RemoteEndPoint.ReceiveTimeout = Timeout;
                RemoteEndPoint.SendTimeout = Timeout;
                RemoteEndPoint.Connect(ConnectRemota);

                var buffer = System.Text.Encoding.Default.GetBytes(ACK);

                Console.WriteLine("ACK -> Envio {0}", ACK);
                RemoteEndPoint.Send(buffer);
                RemoteEndPoint.Receive(buffer);

                var recibedData = System.Text.Encoding.ASCII.GetString(buffer); // Se espera: ACK =  = \u0006
                Console.WriteLine("EOT <- Recibi: {0}", recibedData.ToString());

                if (recibedData.Equals(ACK))
                    return ACK;
                else
                    return null;
            }

        }
    

    }
}

