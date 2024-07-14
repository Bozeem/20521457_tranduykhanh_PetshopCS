using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{
    public partial class Dashboard : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        
        DbConnect dbcon = new DbConnect();
        SqlDataReader dr;
        string title = "Pet Shop Management System";
        public Dashboard()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());           
            LoadCategory();
            LoadSchedule();
        }



        #region method
        public int extractData(string str)
        {
            int data = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT ISNULL(SUM(pqty),0) AS qty FROM tbProduct WHERE pcategory='"+ str +"'", cn);
                data = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
            return data;
        }
        
        public void LoadCategory()
        {
            
            int i = 0;
            dgvCategory1.Rows.Clear();
            cm = new SqlCommand("SELECT C.cateid,C.name,B.qty FROM tbCategory AS C LEFT JOIN(SELECT pcategory, SUM(pqty) AS qty FROM tbProduct GROUP BY pcategory) AS B ON C.cateid = B.pcategory", cn);
            
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                               
                i++;
                dgvCategory1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString()) ;
               chart1.Series["Category"].Points.AddXY(dr[1].ToString(),dr[2].ToString());
            }
            
            
            dr.Close();
            cn.Close();
        }
        public void LoadSchedule()
        {
            int i = 0;
            dgvSchedule.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tbSchedule", cn);
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;

                dgvSchedule.Rows.Add(i, dr[0].ToString(), dr[1].ToString());
            }
            dr.Close();
            cn.Close();
        }

        #endregion method

        private void Dashboard_Load(object sender, EventArgs e)
        {
            lblDog.Text = extractData("1").ToString();
            lblCat.Text = extractData("2").ToString();
            lblbird.Text = extractData("3").ToString();
            lblFish.Text = extractData("4").ToString();
            
        }

        private void BtnPetCategory_Click(object sender, EventArgs e)
        {
            CategoryForm cate = new CategoryForm(this);
            cate.ShowDialog();
        }

        private void dgvCategory1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            lblPet.Text = dgvCategory1.Rows[e.RowIndex].Cells[2].Value.ToString();
            string qan = dgvCategory1.Rows[e.RowIndex].Cells[1].Value.ToString();
            lblpetquan.Text = extractData(qan).ToString();
            

        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            ScheduleModule sche = new ScheduleModule(this);
            sche.ShowDialog();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.ShowDialog();

        }
    }
}
