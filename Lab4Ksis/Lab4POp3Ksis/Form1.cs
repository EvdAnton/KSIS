using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenPop.Mime;
using OpenPop.Pop3;


namespace Lab4POp3Ksis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (Pop3Client client = new Pop3Client())
            {
                client.Connect("pop.gmail.com", 995, true);

                client.Authenticate("*********", "***********", AuthenticationMethod.UsernameAndPassword);

                if (client.Connected) 
                {
                    int messageCount = client.GetMessageCount();

                    List<OpenPop.Mime.Message> allMessages = new List<OpenPop.Mime.Message>(messageCount);


                    for (int i = messageCount; i > 0; i--)
                    {
                        try
                        {
                            allMessages.Add(client.GetMessage(i));

                            OpenPop.Mime.Message message = client.GetMessage(i);

                            string subject = message.Headers.Subject; //заголовок
                            string date = message.Headers.Date.ToString(); //Дата/Время
                            string from = message.Headers.From.ToString(); //от кого
                            string body = "";

                            MessagePart mpPlain = message.FindFirstPlainTextVersion();

                            if (mpPlain != null)
                            {
                                Encoding enc = mpPlain.BodyEncoding;
                                body = enc.GetString(mpPlain.Body); //получаем текст сообщения
                            }

                            ListViewItem mes = new ListViewItem(new string[] { subject, date, from, body });
                            listView1.Items.Add(mes);
                        }
                        catch
                        {
                            MessageBox.Show("Неопознанная кодировка");
                        }

                    }

                  //  for (int i = 0; i < messageCount; i++)
                    //    client.DeleteMessage(messageCount - i);
                }
            }

        }
    }
}

