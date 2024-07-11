using WinFormsApp1.Services;

namespace WinFormsApp1
{
    public partial class MainForm : Form
    {
        private readonly IDataConverter DataConverter;

        public MainForm(IDataConverter converter)
        {
            DataConverter = converter;
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Text = await DataConverter.GetDataTypeMessageAsync("Hello World");
        }
    }
}
