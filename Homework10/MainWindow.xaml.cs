using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
//using TelegramMesageClient;

namespace Homework10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TelegramMessageClient client;
        public MainWindow()
        {
            InitializeComponent();
            client = new TelegramMessageClient(this);
            logList.ItemsSource = client.BotMessageLog;
            
        }

        private void btnMsgSendClick(object sender, RoutedEventArgs e)
        {
            client.SendSMS(txtMsgSend.Text, long.Parse(TargetSend.Text));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XElement xMessages = new XElement("Log");
            foreach(var x in client.BotMessageLog)
            {
                XElement xMessage = new XElement("Message");
                xMessage.Add(new XAttribute("ID", x.ID));
                xMessage.Add(new XAttribute("Nickname", x.FirstName));
                xMessage.Add(new XAttribute("Time", DateTime.Now.ToString()));
                xMessage.Add(new XAttribute("Text", x.Msg));
                xMessages.Add(xMessage);
            }

            xMessages.Save("table.xml");
        }
    }
}
