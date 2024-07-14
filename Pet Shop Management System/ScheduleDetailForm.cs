using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Xml.Linq;
using TheArtOfDevHtmlRenderer.Adapters;

namespace Pet_Shop_Management_System
{
    public partial class ScheduleDetailForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlConnection cn1 = new SqlConnection();
        SqlCommand cm1 = new SqlCommand();

        DbConnect dbcon = new DbConnect();
        string title = "Pet Shop Management System";
        bool check = false;
        ScheduleForm schedule;
        public ScheduleDetailForm(ScheduleForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            cn1 = new SqlConnection(dbcon.connection());
            schedule = form;
            LoadCombobox();
            LoadPetSchedule();            
            cbCategory.SelectedIndex = 0;       
            cbSchedule.SelectedIndex = 0;
        }
 

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                CheckField();
                if (check==true)
                {
                    if (MessageBox.Show("Are you sure you want to Add Schedule?", "Add Schedule", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cm = new SqlCommand("INSERT INTO tbScheduleDetail(pcategory,scid, scdday,scdstatus, scddaycheck)VALUES(@pcategory,@scid, @scdday,@scdstatus, @scddaycheck)", cn);
                        cm.CommandType = CommandType.Text;                        
                        cm.Parameters.AddWithValue("@pcategory", lblpetid.Text);
                        cm.Parameters.AddWithValue("@scid", lblscheid.Text);
                        cm.Parameters.AddWithValue("@scdday", dtDay.Value);
                        cm.Parameters.AddWithValue("@scdstatus", txtStatus.Text);
                        cm.Parameters.AddWithValue("@scddaycheck", dtDayCheck.Value);
                        cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Schedule Detail has been successfully Add!", title);
                        Clear();
                        schedule.LoadSchedule();
                    }

                }

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }
        #region Method
        public void Clear()
        {
            txtStatus.Clear();
            dtDay.Value = DateTime.Now;
            cbCategory.SelectedIndex = 0;
        }

        public void CheckField()
        {            
            DateTime date1 = dtDay.Value;
            int totalDays1 = Convert.ToInt32((date1.Date - DateTime.UtcNow.Date).TotalDays);
            if (txtStatus.Text == "" | totalDays1 < 0 )
            {
                MessageBox.Show("Required data field!", "Warning");
                return;
            }
            check = true;
        }
        public void LoadCombobox()
        {
            cn1.Open();
            cm1 = new SqlCommand("SELECT cateid,name FROM tbCategory", cn1);
            SqlDataReader dr1 = cm1.ExecuteReader();
            DataTable dt1 = new DataTable();
            dt1.Load(dr1);
            cbCategory.ValueMember = "cateid";
            cbCategory.DisplayMember = "name";
            cbCategory.DataSource = dt1;
            dr1.Close();
            cn1.Close();


        }
        public void LoadPetSchedule()
        {
            cn1.Open();
            cm1 = new SqlCommand("SELECT scid,scname FROM tbSchedule", cn1);
            SqlDataReader dr1 = cm1.ExecuteReader();
            DataTable dt1 = new DataTable();
            dt1.Load(dr1);
            cbSchedule.ValueMember = "scid";
            cbSchedule.DisplayMember = "scname";
            cbSchedule.DataSource = dt1;
            dr1.Close();
            cn1.Close();

        }

        #endregion Method

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int petcategory = Convert.ToInt32(cbCategory.SelectedValue.ToString());
            lblpetid.Text = Convert.ToString(petcategory);
        }

        private void cbSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            int scheduel = Convert.ToInt32(cbSchedule.SelectedValue.ToString());
            lblscheid.Text = Convert.ToString(scheduel);
        }
    }
}
