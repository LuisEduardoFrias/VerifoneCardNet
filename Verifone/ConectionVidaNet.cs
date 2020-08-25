

namespace varifoneVisaNet
{

    using System;

    public struct ConectionVisaNet
    {

        #region Property

        private System.IO.Ports.SerialPort _SerialPort;
        private static ConectionVisaNet Instances;
        private bool _loop;
        #endregion


        #region singleton

        public static ConectionVisaNet GetIntances(string port,int speed,System.IO.Ports.Parity parity,int data,System.IO.Ports.StopBits stopBits)
        {
            Instances = new ConectionVisaNet(port,speed,parity,data,stopBits);

            return Instances;
        }

        #endregion



        private ConectionVisaNet(string port,int speed,System.IO.Ports.Parity parity,int databit,System.IO.Ports.StopBits stopBits)
        {
            _SerialPort = new System.IO.Ports.SerialPort(port,speed,parity,databit,stopBits);
            _loop = true;
        }



        public System.IO.Ports.SerialPort OpenComunication()
        {
            try
            {
                _SerialPort.Handshake = System.IO.Ports.Handshake.None;

                _SerialPort.RtsEnable = false;
                _SerialPort.DtrEnable = true;

               
                _SerialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

                _SerialPort.ReadTimeout = 500;
                _SerialPort.WriteTimeout = 500;

                _SerialPort.Open();

                return _SerialPort;
            }
            catch(ArgumentOutOfRangeException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(InvalidOperationException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(ArgumentException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(System.IO.IOException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open Serial Port. " + ex.Message);
            }

            return null;
        }

        public void CloseComunication()
        {
            _SerialPort.Close();
        }

        public System.IO.Ports.SerialPort OpenComunication(System.IO.Ports.Handshake Handshake)
        {
            try
            {
                _SerialPort.Handshake = Handshake;

                _SerialPort.RtsEnable = false;
                _SerialPort.DtrEnable = true;

     
                _SerialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

                _SerialPort.ReadTimeout = 500;
                _SerialPort.WriteTimeout = 500;

                _SerialPort.Open();

                return _SerialPort;
            }
            catch(ArgumentOutOfRangeException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(InvalidOperationException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(ArgumentException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(System.IO.IOException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open Serial Port. " + ex.Message);
            }

            return null;
        }

        public System.IO.Ports.SerialPort OpenComunication(bool RtsEnable,bool DtrEnable)
        {
            try
            {
                _SerialPort.Handshake = System.IO.Ports.Handshake.None;

                _SerialPort.RtsEnable = RtsEnable;
                _SerialPort.DtrEnable = DtrEnable;

               
                _SerialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

                _SerialPort.ReadTimeout = 500;
                _SerialPort.WriteTimeout = 500;

                _SerialPort.Open();

                return _SerialPort;
            }
            catch(ArgumentOutOfRangeException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(InvalidOperationException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(ArgumentException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(System.IO.IOException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open Serial Port. " + ex.Message);
            }

            return null;
        }

        public System.IO.Ports.SerialPort OpenComunication(int ReadTimeout,int WriteTimeout)
        {
            try
            {
                _SerialPort.Handshake = System.IO.Ports.Handshake.None;

                _SerialPort.RtsEnable = false;
                _SerialPort.DtrEnable = true;

               
                _SerialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

                _SerialPort.ReadTimeout = ReadTimeout;
                _SerialPort.WriteTimeout = WriteTimeout;

                _SerialPort.Open();

                return _SerialPort;
            }
            catch(ArgumentOutOfRangeException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(InvalidOperationException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(ArgumentException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(System.IO.IOException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open Serial Port. " + ex.Message);
            }

            return null;
        }

        public System.IO.Ports.SerialPort OpenComunication(System.IO.Ports.Handshake Handshake,bool RtsEnable,bool DtrEnable)
        {
            try
            {
                _SerialPort.Handshake = Handshake;

                _SerialPort.RtsEnable = RtsEnable;
                _SerialPort.DtrEnable = DtrEnable;

               
                _SerialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

                _SerialPort.ReadTimeout = 500;
                _SerialPort.WriteTimeout = 500;

                _SerialPort.Open();

                return _SerialPort;
            }
            catch(ArgumentOutOfRangeException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(InvalidOperationException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(ArgumentException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(System.IO.IOException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open Serial Port. " + ex.Message);
            }

            return null;
        }

        public System.IO.Ports.SerialPort OpenComunication(System.IO.Ports.Handshake Handshake,int ReadTimeout,int WriteTimeout)
        {
            try
            {
                _SerialPort.Handshake = Handshake;

                _SerialPort.RtsEnable = false;
                _SerialPort.DtrEnable = true;

               
                _SerialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

                _SerialPort.ReadTimeout = ReadTimeout;
                _SerialPort.WriteTimeout = WriteTimeout;

                _SerialPort.Open();

                return _SerialPort;
            }
            catch(ArgumentOutOfRangeException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(InvalidOperationException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(ArgumentException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(System.IO.IOException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open Serial Port. " + ex.Message);
            }

            return null;
        }

        public System.IO.Ports.SerialPort OpenComunication(System.IO.Ports.Handshake Handshake, bool RtsEnable, bool DtrEnable, int ReadTimeout, int WriteTimeout)
        {
            try
            {
                _SerialPort.Handshake = Handshake;

                _SerialPort.RtsEnable = RtsEnable;
                _SerialPort.DtrEnable = DtrEnable;

               
                _SerialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

                _SerialPort.ReadTimeout = ReadTimeout;
                _SerialPort.WriteTimeout = WriteTimeout;

                _SerialPort.Open();

                return _SerialPort;
            }
            catch(ArgumentOutOfRangeException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(InvalidOperationException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(ArgumentException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(System.IO.IOException Ex)
            {
                Console.WriteLine("Open Serial Port. " + Ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open Serial Port. " + ex.Message);
            }

            return null;
        }



        private void port_ErrorReceived(object sender,System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("There was an error in the port" + sender.ToString());
        }




        public string SendData(string Trama)
        {
            string Answer = string.Empty;

            try
            {
                byte[] Data = System.Text.Encoding.ASCII.GetBytes(Trama);

                if((_SerialPort != null))
                {
                    _SerialPort.Write(Data,0,Data.Length);
                }

                Answer = "Data Sent.";
                Console.WriteLine("\nData Sent.");

            }
            catch(Exception ex)
            {
                Answer = "Read error :" + ex.Message.ToString() + "\n-Open the Comunication.\n-check COM port (serial port).";
            }

            return Answer;
        }


        public string GetAnswer()
        {
            string Answer = string.Empty;

            try
            {
                Answer = _SerialPort.ReadExisting();
            }
            catch(Exception ex)
            {
                Answer = "Read error :" + ex.Message.ToString();
            }

            return Answer;
        }



        public string SendDataAsync(string Trama,TimeSpan waitTime)
        {
            TimeSpan startToWait;

            string Answer = string.Empty;

            try
            {
                Answer = SendData(Trama);

                startToWait = new TimeSpan(DateTime.Now.Hour,DateTime.Now.Second,DateTime.Now.Millisecond);

                if(Answer.Substring(0,10) == "Data Sent.")
                {
                    int accountant1 = 0, accountant2 = 0, accountant3 = 0;

                    while(true)
                    {

                        Answer = GetAnswer();

                        if(Answer != Convert.ToString("\u0006"))
                        {
                            if(Answer != string.Empty)
                            {

                                if(Answer == Convert.ToString("\u000299\u0003") || Answer == Convert.ToString("\u000299\u0003\u0006"))
                                {
                                    return "99";
                                }
                                else
                                {
                                    return Answer.Substring(1);
                                }

                            }
                        }

                        TimeSpan residuo = startToWait.Subtract(waitTime);

                        if(residuo.TotalMinutes >= 2)
                            return "Time is up";


                        waiting(ref accountant1,ref accountant2,ref accountant3);
                    }

                }
                else
                   return Answer;

            }
            catch(Exception ex)
            {
                Answer = "Read error :" + ex.Message.ToString();
            }

            return Answer;
        }

        private void waiting(ref int accountant1,ref int accountant2,ref int accountant3)
        {
            if(accountant1 == 0)
            {
                Console.Write("\nwaiting for your reply");
                accountant1 = 1;
            }

            if(accountant2 == 100000)
            {
                Console.Write(".");
                accountant3++;
                accountant2 = 0;
            }

            if(accountant3 == 3)
            {
                accountant3 = 0;
                accountant1 = 0;
            }

            accountant2++;
        }


    }

}
