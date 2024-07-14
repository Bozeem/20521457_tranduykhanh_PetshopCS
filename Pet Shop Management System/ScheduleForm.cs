using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using static Guna.UI2.Native.WinApi;
using System.Xml.Linq;

namespace Pet_Shop_Management_System
{
    public partial class ScheduleForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbcon = new DbConnect();
        SqlDataReader dr;
        bool check = false;
        string title = "Pet Shop Management System";
        public ScheduleForm()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            LoadSchedule();
            LoadPetSchedule();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                CheckField();
                if (check == true)
                {
                    if (MessageBox.Show("Are you sure you want to update this Schedule detail?", "Update Schedule", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cm = new SqlCommand("UPDATE tbScheduleDetail SET scid=@scid, scdstatus=@scdstatus,scdday=@scdday,scddaycheck=@scddaycheck WHERE scdid=@scdid", cn);
                        cm.CommandType = CommandType.Text;
                        cm.Parameters.AddWithValue("@scid", lblScheId.Text); 
                        cm.Parameters.AddWithValue("@scdstatus", txtStatus.Text); 
                        cm.Parameters.AddWithValue("@scdday", dtDaySche.Value);
                        cm.Parameters.AddWithValue("@scddaycheck", DateTime.Now); // luu ngay update la thoi diem hien tai nhan update
                        cm.Parameters.AddWithValue("@scdid", lblScdid.Text);
                        cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                        LoadSchedule();
                        clear();
                        MessageBox.Show("Schedule data has been successfully update", title, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }

        }

        #region method
        public void LoadPetSchedule() // load schedule len combobox
        {
            cn.Open();
            cm = new SqlCommand("SELECT scid,scname FROM tbSchedule", cn);
            SqlDataReader dr1 = cm.ExecuteReader();
            DataTable dt1 = new DataTable();
            dt1.Load(dr1);
            cbSchedule.ValueMember = "scid";
            cbSchedule.DisplayMember = "scname";
            cbSchedule.DataSource = dt1;
            dr1.Close();
            cn.Close();

        }
        public void LoadSchedule() // lay tu database load len datagridview
        { 
            int i = 0;
            dgvSchedule.Rows.Clear();
            // ket 3 bang lai de lay ten theo ma category, schedule
            cm = new SqlCommand("SELECT scdid,c.name,sc.scname,scdday,scdstatus,scddaycheck FROM tbScheduleDetail as sche Left JOIN tbCategory c ON sche.pcategory = c.cateid LEFT JOIN tbSchedule sc ON sche.scid = sc.scid WHERE CONCAT(scdid,c.name,sc.scname,scdday,scdstatus,scddaycheck) LIKE '%" + txtSearch.Text + "%'", cn); 
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvSchedule.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), DateTime.Parse(dr[3].ToString()).ToShortDateString(), dr[4].ToString(), DateTime.Parse(dr[5].ToString()).ToShortDateString());
            }
            dr.Close();
            cn.Close();
        }
        public void selctCateID(string scheID) // lay cateid tu database load len lblCateID de update
        {
            cm = new SqlCommand("SELECT pcategory FROM tbScheduleDetail WHERE scdid=@id ", cn);
            cn.Open();
            cm.Parameters.AddWithValue("@id", scheID);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                lblCateID.Text = dr["pcategory"].ToString();

            }

            dr.Close();
            cn.Close();
        }
        public void clear()
        {           
           

        }
        public void CheckField()
        {
            DateTime date1 = dtDaySche.Value;
            int totalDays1 = Convert.ToInt32((date1.Date - DateTime.UtcNow.Date).TotalDays);
            if (txtStatus.Text == "" | totalDays1 < 0) // neu thoi gian nho hon hien tai thi khong dc
            {
                MessageBox.Show("Required data field!", "Warning");
                return;
            }
            check = true;
        }
        #endregion method

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //string colName = dgvSchedule.Columns[e.ColumnIndex].Name;
            //if (colName == "delete")
            //{
            //    if (MessageBox.Show("Are you sure you want to delete this items?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
                    
            //        dbcon.executeQuery("DELETE FROM tbSchedule WHERE scid LIKE '" + dgvSchedule.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
            //        MessageBox.Show("Item record has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //}
            //LoadSchedule();
        }
        
        private void dgvSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            DataGridViewRow row = this.dgvSchedule.CurrentRow;
            selctCateID(row.Cells["Column4"].Value.ToString());
            lblScdid.Text = dgvSchedule.Rows[e.RowIndex].Cells[1].Value.ToString();
            lblCateID.Visible=true;
            lblPCategory.Text = row.Cells["dataGridViewTextBoxColumn3"].Value.ToString();
            lblPCategory.Visible = true;
            cbSchedule.Text = dgvSchedule.Rows[e.RowIndex].Cells[3].Value.ToString();
            cbSchedule.Visible = true;
            lblScheId.Visible= true;
            DateTime dt = DateTime.Parse(row.Cells["Column1"].Value.ToString());
            int totalHours = Convert.ToInt32((dt.Date - DateTime.UtcNow.Date).TotalHours);
            lblAction.Text = totalHours.ToString();
            lblAction.Visible = true;
            DateTime dt1 = DateTime.Parse(dgvSchedule.Rows[e.RowIndex].Cells[6].Value.ToString());
            int totalHours1 = Convert.ToInt32((DateTime.UtcNow.Date-dt1.Date).TotalHours);
            lblChecker.Text = totalHours1.ToString();
            lblChecker.Visible = true;           
            txtStatus.Text = row.Cells["Column2"].Value.ToString();
            dtDaySche.Text = dgvSchedule.Rows[e.RowIndex].Cells[4].Value.ToString(); 
            btnUpdate.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ScheduleDetailForm schedt = new ScheduleDetailForm(this);
            schedt.ShowDialog();
        }

        private void cbSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            int scheduel = Convert.ToInt32(cbSchedule.SelectedValue.ToString());
            lblScheId.Text = Convert.ToString(scheduel);
        }
    }
}
