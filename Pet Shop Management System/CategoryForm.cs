using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{
    public partial class CategoryForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbcon = new DbConnect();
        SqlDataReader dr;
        bool check = false;
        Dashboard dashboard;
        string title = "Pet Shop Management System";
        public CategoryForm(Dashboard dash)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            dashboard = dash;
            LoadCategory();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CheckField();
            if (check) 
            { 
                if (checkCategory(lblcid.Text)==false)
                { 
                cm = new SqlCommand("INSERT INTO tbCategory(name)VALUES(@name)", cn);            
                cm.Parameters.AddWithValue("@name", txtName.Text);       
                cn.Open();
                cm.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("Pet Category has been successfully registered!", title);           
                LoadCategory();
                }
                else if (checkCategory(lblcid.Text)==true)
                {
                    cm = new SqlCommand("UPDATE tbCategory SET name=@name WHERE cateid=@cateid", cn);
                    cm.Parameters.AddWithValue("@cateid", lblcid.Text);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cn.Open();
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Pet Category has been successfully update!", title);
                    LoadCategory();
                }
                
            }
            Clear();

        }
      

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCategory();
        }

        #region method
       
        public void LoadCategory()
        {
            int i = 0;
            dgvCategory.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tbCategory WHERE CONCAT(cateid,name) LIKE '%" + txtSearch.Text + "%'", cn);
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvCategory.Rows.Add(i, dr[0].ToString(), dr[1].ToString());
            }
            dr.Close();
            cn.Close();
        }
        public bool checkCategory(string Ccode)
        {
            bool flag = false;
            cn.Open();
            string sql = "SELECT cateid FROM tbCategory WHERE cateid='" + Ccode + "'";
            SqlDataAdapter dap = new SqlDataAdapter(sql, cn);
            DataTable table = new DataTable();
            dap.Fill(table);
            if (table.Rows.Count > 0)
                flag = true;
            cn.Close();
            return flag;
        }
        public void CheckField()
        {
            if (txtName.Text == "" )
            {
                MessageBox.Show("Required data field!", "Warning");
                return;
            }
            check = true;
        }
        public bool checkPcate(string pcategory)
        {
            bool flag = false;
            cn.Open();
            string sql = "SELECT pcategory FROM tbSchedule WHERE pcategory='" + pcategory + "'";
            SqlDataAdapter dap = new SqlDataAdapter(sql, cn);
            DataTable table = new DataTable();
            dap.Fill(table);
            if (table.Rows.Count > 0)
                flag = true;
            cn.Close();
            return flag;
        }
        public void Clear()
        {
            lblcid.Text = "0";
            txtName.Clear();
        }
        #endregion method

        private void dgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            lblcid.Text = dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtName.Text = dgvCategory.Rows[e.RowIndex].Cells[2].Value.ToString();
        }

        private void dgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvCategory.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this items?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dbcon.executeQuery("DELETE FROM tbScheduleDetail WHERE pcategory LIKE '" + dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    dbcon.executeQuery("DELETE FROM tbCategory WHERE cateid LIKE '" + dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                   
                    MessageBox.Show("Item record has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            LoadCategory();
            Clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //foreach (DataGridViewRow dr in dgvCategory.Rows)
            //{
                
            //        if (checkPcate(dr.Cells[1].Value.ToString()) == false)
            //        {
            //            try
            //            {
            //                cm = new SqlCommand("INSERT INTO tbSchedule(pcategory,scfood,schealth, schygienpet,schygienhome )VALUES(@pcategory,@scfood,  @schealth, @schygienpet,@schygienhome)", cn);
            //                cm.CommandType = CommandType.Text;
            //                cm.Parameters.AddWithValue("@pcategory", dr.Cells[1].Value.ToString());
            //                cm.Parameters.AddWithValue("@scfood", DateTime.Now);
            //                cm.Parameters.AddWithValue("@schealth", DateTime.Now);
            //                cm.Parameters.AddWithValue("@schygienpet", DateTime.Now);
            //                cm.Parameters.AddWithValue("@schygienhome", DateTime.Now);
                             
            //                string pcategory = dr.Cells[1].Value.ToString();
            //                cn.Open();
            //                cm.ExecuteNonQuery();
            //                cn.Close();
            //            }
            //            catch (Exception ex)
            //            {
            //                cn.Close();
            //                MessageBox.Show(ex.Message, title);
            //            }
            //        }
                
            //}
        }
    }
}
