using System;
using System.Windows.Forms;

namespace LSB_GUI
{
    
    public partial class Form2 : Form
    {
        private readonly Form1 mainForm;
        public StatisticalCharacteristics StatChar= new StatisticalCharacteristics();
        public Form2(Form1 mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                RGBTRIPLE maxDifference = StatChar.getMaxDifference(mainForm.ContainerForEncode, mainForm.ContainerForDecode);
                string maxDifferenceRed = Convert.ToString(maxDifference.rgbtRed, 2).PadLeft(8,'0');
                string maxDifferenceGreen = Convert.ToString(maxDifference.rgbtGreen, 2).PadLeft(8,'0');
                string maxDifferenceBlue = Convert.ToString(maxDifference.rgbtBlue, 2).PadLeft(8,'0');
                textBox1.Text = "Red: " + maxDifferenceRed + " Green: " + maxDifferenceGreen + " Blue: " + maxDifferenceBlue;
            }
            catch (Exception excptn)
            {
                MessageBox.Show("Анта бака!\nОшибка при подсчете характеристик!\n" + excptn.StackTrace, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}