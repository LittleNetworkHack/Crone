using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
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
            //TestSqlCommand();
            TestOraCommand();
        }

        void TestSqlCommand()
		{
            using var connection = new SqlConnection(ExampleClass.AdventureDB);
            using var command = new Database.ExampleSqlCommand(connection)
            {
                FirstName = "Amy%"
            };
            using var reader = new CoreDataReader(command);

            reader.Read();

            var x = reader.GetDictionary();
        }

        void TestOraCommand()
        {
            using var connection = new OracleConnection(ExampleClass.OracleHRDB);
            using var command = new Database.ExampleOraCommand(connection)
            {
                //FirstName = "A%",
                Salary = 5000
            };
            using var reader = new CoreDataReader(command);

            reader.Read();

            var x = reader.GetDictionary();
        }
    }
}
