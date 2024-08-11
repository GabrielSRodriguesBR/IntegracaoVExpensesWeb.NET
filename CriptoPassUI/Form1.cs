using CriptoPass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CriptoPassUI
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string password = textBox1.Text;
			if (!String.IsNullOrEmpty(password))
			{
				string criptoPass = CriptoPass.CriptoPass.CriptografarSenha(password);
				Clipboard.SetText(criptoPass);
				textBox2.Text = criptoPass;
				MessageBox.Show($"Senha gerada: {criptoPass}", "Copiado para área de transferência", MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
		}
	}
}
