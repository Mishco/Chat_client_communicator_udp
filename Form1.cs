using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections;
using System.Threading;

namespace PKS_2_zadanie_KLIENT
{
    public partial class Form1 : Form
    {
        //potrebne na komunikaciu
        Socket socketCommunication;
        EndPoint epLocal, epRemote;

        Worker workerObject;
        Thread workerThread;

        Worker receiveObject;
        Thread receiveThread;

        private BackgroundWorker bw = new BackgroundWorker();

        bool sending = false;
        bool listening = false;
        bool printing = false;
        int Count = 0;

        int fragmentSize = 1464; //1464 ???????

        byte[] buffer;

        // Creates and initializes a new ArrayList.
        ArrayList allMsg = new ArrayList();

        //v hlavice stati USHORT, ktory ma 2 bajty
        int head_PoradCislo = 0; // 65,535 maximum
        int head_PosledneCislo = 0;

        public Form1()
        {
            InitializeComponent();

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();// vrati aktualnu adresu
                }
            }
            return "127.0.0.1"; // inac vrat localhost ...teda pracujeme na jednom pc
        }

        // run 
        private void button1_Click(object sender, EventArgs e)
        {
            // moje udaje, IP adresa a port
            epLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text));
            socketCommunication.Bind(epLocal);  // binding

            // pripoj sa na cudzieho IP a port 
            epRemote = new IPEndPoint(IPAddress.Parse(textBox4.Text), Convert.ToInt32(textBox5.Text));
            socketCommunication.Connect(epRemote);

            // zacni pocuvat specificky PORT a IP
            buffer = new byte[1464];
            socketCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                    ref epRemote, new AsyncCallback(MessageCall), buffer);

            button2.Enabled = true; // mozeme posielat
         //  button1.Text = "spojene"; //label nastavit na spojene
           // button1.Enabled = false;

        }




        private void MessageCall(IAsyncResult ar)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            
            byte[] res;
            byte a = 0, b = 0, c = 0, d = 0;

            int secondNumber = 0;
            int fixedOffset = 8; // 4 + 4 cela hlavicka

            string  druhe;

            try
            {
                if (sending == false)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                if (sending == true && listening == false)
                {
                    //if (receiveThread.IsAlive == false)
                    //    receiveThread.Start();
                    
                    listening = true;
                }

                int size = socketCommunication.EndReceiveFrom(ar, ref epRemote);
                // zisti velksot a riesi fragmentaciu 
                if (size > fragmentSize && size != 0)
                {
                    //treba ich spajat a teda cakat kym pridu vsetky
                    res = (byte[])ar.AsyncState; // zmeni VELKOST !!!!!

                    allMsg.Add(res);
                    a = res[4]; b = res[5]; c = res[6]; d = res[7];
                    druhe = Convert.ToString(a) + Convert.ToString(b) + Convert.ToString(c) + Convert.ToString(d);
                    secondNumber = Convert.ToInt32(druhe);
                    if (allMsg.Count < secondNumber)
                    {
                      //workerObject.RequestStop();
                      //  workerThread.Join();

                        if (bw.IsBusy != true)
                        {
                            bw.RunWorkerAsync();
                        }
                        buffer = new byte[1464];
                        socketCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                                ref epRemote, new AsyncCallback(MessageCall), buffer);
                      
                        System.Threading.Thread.Sleep(2000);
                    }

                   
                    // prestan pocuvat a zacni spracuvavat prijate spravy
                    // killing vsetky ostatne thready, ktore nepotrebujem 
                  
                    // workerObject.RequestStop();
                  //  if (workerThread.IsAlive == true)
                   // {
                   //     workerThread.Abort();
                   // }
                   if (printing == false)
                    {
                        if (allMsg.Count == secondNumber)
                        {
                            printing = true;
                            receiveThread.Abort();
                            PrintReceivingMsg(allMsg, allMsg.Count, size, fixedOffset);
                        }
                   }
                }
               


              //  workerThread.Abort();
             //   if (workerThread.IsAlive == true)
             //   {
             //      workerThread.Abort();
             //   }
                if (printing == false)
                {
                    PrintOneMsg(size, ar, fixedOffset);
                }
                // zacni pocuvat od znova
                buffer = new byte[1464];
                socketCommunication.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                        ref epRemote, new AsyncCallback(MessageCall), buffer);
            }

            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString()); // v pripade chyby vypise vynimku
                try
                {
                    socketCommunication.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            
            }

            SetText("------------------------------\n");
            sending = false;
            listening = false;
            printing = false;

         //   workerObject = new Worker();
        //    workerThread = new Thread(workerObject.DoWork);

         //   receiveObject = new Worker();
        //    receiveThread = new Thread(receiveObject.DoWork);
            
            //allMsg.Clear();
            allMsg = new ArrayList();
            
            return;

        }
        delegate void SetTextCallBack(string text);

        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallBack d = new SetTextCallBack(SetText);
                this.Invoke(d, new object[] { text });
            }
            else {
                //this.listBox1.Text = text;
                this.listBox1.Items.Add(text);
            }
        
        }

        //samostatna funkcia na vypisanie prijatej spravy
        public void PrintReceivingMsg(ArrayList messages, int length, int size, int fixedOffset)
        {
            string prve;
            byte a, b, c, d;
            string[] zoradenePole = new string[length];
            int i, index, j;
            int firstNum;
            byte[] onlyMsg;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            string vypis = null;

            for (j = 0; j < messages.Count; j++)
            {
                byte[] tmpMsg = new byte[size];
                object o = messages[j];
                tmpMsg = (byte[])o;
                //zoradanie podla poradia
                a = tmpMsg[0]; b = tmpMsg[1]; c = tmpMsg[2]; d = tmpMsg[3];
                prve = Convert.ToString(a) + Convert.ToString(b) + Convert.ToString(c) + Convert.ToString(d);
                firstNum = Convert.ToInt32(prve); // zisti index na ktoru poziciu ma zoradovat

                ///odstranenie hlavicky
                onlyMsg = new byte[size - fixedOffset];
                index = 0;
                for (i = fixedOffset; i < (size); i++) // skopirovat iba spravu bez hlavicky
                {
                    onlyMsg[index++] = tmpMsg[i];
                }
                string tmp = enc.GetString(onlyMsg);
                zoradenePole[firstNum - 1] = tmp;
            }

           // listBox1.Items.Add("Priatel =  ");
            vypis = "Priatel = ";
            string celaSprava = null;
            for (i = 0; i < zoradenePole.Length; i++)
            {
                //Console.WriteLine(zoradenePole[i] + " " + (i + 1) + " z " + allMsg.Count);
                //listBox1.Items.Add(zoradenePole[i] + " " + (i + 1) + " z " + allMsg.Count);
                vypis = vypis + zoradenePole[i] + " " + (i + 1) + "|" + allMsg.Count + " \n";
                celaSprava = celaSprava + zoradenePole[i]; 
            }
            SetText(vypis + "\n");
            SetText(celaSprava);
            SetText("------------------------\n");
            return;
        }
        public void PrintOneMsg(int size, IAsyncResult ar, int fixedOffset)
        {
            int i, index;
            byte[] res;
            byte[] aux;
            byte[] onlyMsg;
            byte a, b, c, d;
            string msg;
            string prve, druhe;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            int firstNum, secondNumber;
            string vypis;

            //pomocne pole do ktoreho si ulozime ziskanu spravu
            res = new byte[size];
            aux = new byte[size];

            // ziskaj data
            res = (byte[])ar.AsyncState; // zmeni VELKOST !!!!!

            //osekaj nepotrebnu cast
            for (i = 0; i < size; i++)
            {
                aux[i] = res[i];
            }
            a = aux[0];
            b = aux[1];
            c = aux[2];
            d = aux[3];

            prve = Convert.ToString(a) + Convert.ToString(b) + Convert.ToString(c) + Convert.ToString(d);
            firstNum = Convert.ToInt32(prve);
            Console.Write("ord:{0} ", firstNum);
            a = aux[4];
            b = aux[5];
            c = aux[6];
            d = aux[7];

            druhe = Convert.ToString(a) + Convert.ToString(b) + Convert.ToString(c) + Convert.ToString(d);

            secondNumber = Convert.ToInt32(druhe);
            Console.Write("last:{0} ", secondNumber);

            onlyMsg = new byte[size - fixedOffset];
            index = 0;
            for (i = fixedOffset; i < (size); i++) // skopirovat iba spravu bez hlavicky
            {
                onlyMsg[index++] = aux[i];
            }
            // prechod na string
            msg = enc.GetString(onlyMsg);

            // pridaj do vypis
           // listBox1.Items.Add("Priatel =  " + msg + " " + firstNum + " z " + secondNumber);
            vypis = "Priatel =  " + msg + " " + firstNum + "|" + secondNumber + "\n";
            SetText(vypis);
        }
        private void LOADStarting(object sender, EventArgs e)
        {
            int i;
            socketCommunication = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //nastavi komunikaciu na dane IP adresu, a protocol
            socketCommunication.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // zisti moju IP adresu 
            textBox1.Text = GetLocalIP();
            textBox4.Text = GetLocalIP();

            //napln comboBox
            string some;
            for (i = 1; i < 1465; i++)
            {
                some = i.ToString();
                comboBox1.Items.Add(some);
            }
            comboBox1.SelectedValue = 1464;
            label10.Text = "1464"; // na zaciatku defaultne najvacsia 

            textBox2.Text = "9876";//natvrdo dane PORTY
            textBox5.Text = "9876";

            // Create the thread object. This does not start the thread.
            workerObject = new Worker();
            workerThread = new Thread(workerObject.DoWork);

            receiveObject = new Worker();
            receiveThread = new Thread(receiveObject.DoWork);
        }
        //samotny SEND
        private void button2_Click(object sender, EventArgs e)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            int length = enc.GetByteCount(textBox3.Text); // metoda vracajuca presny pocet bajtov potrebnych

            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }

            //zisti ci je potrebna fragmentacia podla nastavenia
            if (length > fragmentSize)
            {
               /* if (workerThread.IsAlive == false)
                {
                    Console.WriteLine("starting worker thread");
                    workerThread.Start();
                }*/

                byte[] msg = new byte[fragmentSize + 4 + 4];

                string celaSprava = textBox3.Text;

                //je potrebna fragmentacia
                // pocet vsetkych soketov
                int countOfsockets = length / fragmentSize;
                Console.WriteLine("pocet fragmentov : {0}", countOfsockets);
                int i;

                head_PosledneCislo = countOfsockets; // nebude viac ako 65 555
                head_PoradCislo = 0;

                int aktualSize = 0;

                for (i = 0; i < countOfsockets; i++) // posielanie po segmentoch 
                {
                    //POSLI SPRAVU
                    byte[] order = new byte[4]; //  prva cast hlavicky, ktora sa meni
                    byte[] last = new byte[4];  // druha cast hlavicky, BEZ ZMENY

                    string upravenaSprava = celaSprava.Substring(aktualSize, fragmentSize);
                    // celaSprava.Split()
                    aktualSize += fragmentSize;

                    order = BitConverter.GetBytes(++head_PoradCislo);
                    Array.Reverse(order); // otoci vypisane cisla 
                    string hlava1 = System.Text.Encoding.UTF8.GetString(order);

                    last = BitConverter.GetBytes(head_PosledneCislo);
                    Array.Reverse(last); // otoci vypisane cisla 
                    string hlava2 = System.Text.Encoding.UTF8.GetString(last);

                    upravenaSprava = hlava1 + hlava2 + upravenaSprava;

                    msg = enc.GetBytes(upravenaSprava); //prelozenie do bajtov

                  //  receiveThread.Interrupt();
                    socketCommunication.Send(msg); // a odoslanie
                    //druhe vlakno nech caka, kym poslem vsetky
                }


               // workerObject.RequestStop();
                sending = true; //poslal som vsetko
                string vypis = "JA = " + celaSprava + "\n";
                //listBox1.Items.Add("JA  = ");
                //listBox1.Items.Add(celaSprava);
                SetText(vypis);
                
                // vycisti po odoslani aj prijati textPole
                textBox3.Clear();
              //  if (workerThread.IsAlive == true)
              //  {
              //      workerThread.Abort();
             //   }
                if (bw.WorkerSupportsCancellation == true)
                {
                    bw.CancelAsync();
                }

                return;
            }
            else  //nie je potrebna fragmentacia, ale hlavicku PRIDAJ s udajmi 
            {
              //  workerObject.RequestStop();
                //---------------POSLI SPRAVU-------------------------------------------------------------------
                // netreba segmentovat posle  1:1
                byte[] msg = new byte[length + 4 + 4];

                string celaSprava = textBox3.Text;
                int intValue = 1;
                byte[] intBytes = BitConverter.GetBytes(intValue);
                Array.Reverse(intBytes); // otoci vypisane cisla 

                string hlava = System.Text.Encoding.UTF8.GetString(intBytes);

                celaSprava = hlava + hlava + celaSprava;
                msg = enc.GetBytes(celaSprava); //prelozenie do bajtov
                socketCommunication.Send(msg); // a odoslanie

                // add to listbox
                //listBox1.Items.Add("JA  = " + textBox3.Text);
                SetText("JA = " + textBox3.Text);
                
                // vycisti po odoslani aj prijati textPole
                textBox3.Clear();
              //  if (workerThread.IsAlive == true)
             ////   {
              //      workerThread.Abort();
             //   }
            }
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
            return;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_Nastav_Click(object sender, EventArgs e)
        {
            fragmentSize = Convert.ToInt32(comboBox1.Text);
            label10.Text = comboBox1.Text;
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 1; (i <= 10); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(500);
                    worker.ReportProgress((i * 10));
                }
            }
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                Console.WriteLine("canceled !");
                //this.tbProgress.Text = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                Console.WriteLine("Error " + e.Error.Message);
                //this.tbProgress.Text = ("Error: " + e.Error.Message);
            }

            else
            {
                Console.WriteLine("Done");
                //this.tbProgress.Text = "Done!";
            }
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage.ToString() + "%");
            //this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
        }

    }


    public class Worker
    {
        public void DoWork()
        {
            while (!_shouldStop)
            {
                Console.WriteLine("worker thread: working...");
            }
            Console.WriteLine("worker thread: terminating gracefully.");
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        private volatile bool _shouldStop;
    }
}
