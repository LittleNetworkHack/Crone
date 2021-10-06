using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crone.Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnTest.Click += TestClick;
            //btnTest.Click += (s, e) => ExampleClass.TestAll();
        }

        void TestClick(object sender, EventArgs args)
		{
            using var connection = new SqlConnection(ExampleClass.AdventureDB);
            using var command = new Database.ExampleCommand(connection)
            {
                FirstName = "Amy%"
            };
            using var reader = new CoreDataReader(command);

            reader.Read();
		}
	}
}
