using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Telemeter_v2
{
    public partial class AddUser : Form
    {
        Encryptor encryptor;
        Telemeter telemeter;

        public AddUser(Telemeter telemeter)
        {
            InitializeComponent();
            this.telemeter = telemeter;
            this.TopMost = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            encryptor = new Encryptor();
            Properties.Settings.Default.username = txtUsername.Text;
            Properties.Settings.Default.password = encryptor.encrypt(txtPassword.Text);
            Properties.Settings.Default.Save();
            MessageBox.Show("User added successfully", "User added", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
            telemeter.GetData();
            this.Dispose();
        }
    }
}
