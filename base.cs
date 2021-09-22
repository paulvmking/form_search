using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace CarsDatabase
{
    public partial class frmCars : Form
    {
        public frmCars()
        {
            InitializeComponent();
        }

        public SQLiteConnection myDbConnection = new SQLiteConnection(@"data source = C:\Users\paulv\source\repos\CarsDatabase\bin\hire.db");

        DataTable tblCar = new DataTable();

        public int currentRowSelect = 0;
        public int currentTableSize = 0;
        public string uid = "";


        private void frmCars_Load(object sender, EventArgs e)
        {
            string mySQLStatement = "SELECT * FROM tblCar";
            myDbConnection.Open();

            SQLiteDataAdapter adapter = new SQLiteDataAdapter(mySQLStatement, myDbConnection);

            tblCar.Clear();

            adapter.Fill(tblCar);

            myDbConnection.Close();

            LoadDataFromDb();
        }

        private void LoadDataFromDb()
        {
            string sqlSelectQuery = $"SELECT * FROM tblCar ORDER BY VehicleRegNo";

            myDbConnection.Open();
            SQLiteDataAdapter da = new SQLiteDataAdapter(sqlSelectQuery, myDbConnection);

            tblCar.Clear();
            da.Fill(tblCar);
            myDbConnection.Close();

            carRegFromDb.Text = tblCar.Rows[currentRowSelect]["VehicleRegNo"].ToString();
            carMakeFromDb.Text = tblCar.Rows[currentRowSelect]["Make"].ToString();
            engineSizeFromDb.Text = tblCar.Rows[currentRowSelect]["EngineSize"].ToString();
            dateRegFromDb.Text = tblCar.Rows[currentRowSelect]["DateRegistered"].ToString();
            rentalPerDayFromDb.Text = tblCar.Rows[currentRowSelect]["RentalPerDay"].ToString();
            availableFromDb.Checked = tblCar.Rows[currentRowSelect]["Available"].ToString() == "0" ? false : true;

            uid = tblCar.Rows[currentRowSelect]["uid"].ToString();
            currentTableSize = tblCar.Rows.Count;
            paginationCurrent.Text = $"{currentRowSelect + 1} of {currentTableSize}";

            
        }

        private void firstNavBtn_Click(object sender, EventArgs e)
        {
            currentRowSelect = 0;
            LoadDataFromDb();
        }

        private void lastNavBtn_Click(object sender, EventArgs e)
        {
            currentRowSelect = currentTableSize - 1;
            LoadDataFromDb();
        }

        private void nextNavBtn_Click(object sender, EventArgs e)
        {
            if (currentRowSelect != currentTableSize - 1)
            {
                currentRowSelect++;
                LoadDataFromDb();
            }
        }

        private void previousNavBtn_Click(object sender, EventArgs e)
        {
            if (currentRowSelect != 0)
            {
                currentRowSelect--;
                LoadDataFromDb();
            }
        }

        private void updateDbBtn_Click(object sender, EventArgs e)
        {
            int isAvailable = availableFromDb.Checked ? 1 : 0;

            DialogResult updateChoice = MessageBox.Show("Are you sure you want to update the selected record with the entered values?", "Update Confirmation", MessageBoxButtons.YesNo);
            if (updateChoice.Equals(DialogResult.Yes))
            {
                string sqlUpdateQuery =
                 $"UPDATE tblCar " +
                 $"SET VehicleRegNo = '{carRegFromDb.Text}', " +
                 $"Make = '{carMakeFromDb.Text}', " +
                 $"EngineSize = '{engineSizeFromDb.Text}', " +
                 $"DateRegistered = '{dateRegFromDb}', " +
                 $"RentalPerDay = '{rentalPerDayFromDb}', " +
                 $"Available = '{isAvailable}', " +
                 $"Where uid = '{uid}', ";

                myDbConnection.Open();
                SQLiteCommand sqlCommand = new SQLiteCommand(sqlUpdateQuery, myDbConnection);

                sqlCommand.ExecuteNonQuery();
                myDbConnection.Close();

                MessageBox.Show("Record Updated Successfully!");

                currentRowSelect = 0;
                LoadDataFromDb();
            }
            else if (updateChoice.Equals(DialogResult.No))
            {
                MessageBox.Show("No changes made to record, press ok to return.", "Update action cancelled");
            }
        }

        private void addToDbBtn_Click(object sender, EventArgs e)
        {
            int isAvailable = availableFromDb.Checked ? 1 : 0;

            string sqlAddQuery =
                 $"INSERT INTO tblCar(VehicleRegNo, Make, EngineSize, DateRegistered, RentalPerDay, Available, uid) values ( " +
                 $"'{carMakeFromDb.Text}', " +
                 $"'{engineSizeFromDb.Text}', " +
                 $"'{dateRegFromDb}', " +
                 $"'{rentalPerDayFromDb}', " +
                 $"'{isAvailable}', " +
                 $"'{uid}' )";

            myDbConnection.Open();
            SQLiteCommand sqlCommand = new SQLiteCommand(sqlAddQuery, myDbConnection);

            sqlCommand.ExecuteNonQuery();
            myDbConnection.Close();

            MessageBox.Show("Record Added Successfully!");

            currentRowSelect = 0;
            LoadDataFromDb();
        }

        private void deleteFromDbBtn_Click(object sender, EventArgs e)
        {
            DialogResult deleteChoice = MessageBox.Show("Are you sure you want to permanently delete the selected record?", "Delete Confirmation", MessageBoxButtons.YesNo);
                if (deleteChoice.Equals(DialogResult.Yes))
            {
                string sqlDeleteQuery =
                    $"DELETE FROM tblCar " +
                    $"WHERE uid = '{uid}'";

                myDbConnection.Open();
                SQLiteCommand sqlCommand = new SQLiteCommand(sqlDeleteQuery, myDbConnection);

                sqlCommand.ExecuteNonQuery();
                myDbConnection.Close();

                MessageBox.Show("Record Deleted Successfully!");

                currentRowSelect = 0;
                LoadDataFromDb();
            } else if (deleteChoice.Equals(DialogResult.No))
            {
                MessageBox.Show("No changes made to record, press ok to return.", "Delete action cancelled");
            }

        }

        private void searchDbBtn_Click(object sender, EventArgs e)
        {
            frmSearch searchForm = new frmSearch();
            searchForm.Show();
        }

        private void cancelActionBtn_Click(object sender, EventArgs e)
        {

        }

        private void exitAppBtn_Click(object sender, EventArgs e)
        {
            DialogResult exitChoice = MessageBox.Show("Are you sure you want to exit the application?", "Exit application", MessageBoxButtons.YesNo);
            if (exitChoice.Equals(DialogResult.Yes))
            {
                MessageBox.Show("Application closed Successfully!", "Exit Application");
                Close();
            }
            else if (exitChoice.Equals(DialogResult.No))
            {
                MessageBox.Show("You have chosen to keep the application open.", "Application running");
                LoadDataFromDb();
            }
        }
    }
}