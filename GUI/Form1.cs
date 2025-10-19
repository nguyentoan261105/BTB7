using BUS;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace GUI
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        private string selectedImagePath = null;
        public frmStudent()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            btnThemSua.Click += btnThemSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnOpen.Click += btnOpen_Click;
            chkChuaDK.CheckedChanged += chkChuaDK_CheckedChanged;
            dgvQLSV.SelectionChanged += dgvQLSV_SelectionChanged;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadFacultyList();
                LoadStudentList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void LoadFacultyList()
        {
            var listFac = facultyService.GetAll();
            cmbKhoa.DataSource = listFac;
            cmbKhoa.DisplayMember = "FacultyName";
            cmbKhoa.ValueMember = "FacultyID";
        }

        private void LoadStudentList()
        {
            var listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void BindGrid(List<Student> list)
        {
            dgvQLSV.Rows.Clear();
            foreach (var s in list)
            {
                int index = dgvQLSV.Rows.Add();
                dgvQLSV.Rows[index].Cells[0].Value = s.StudentID;
                dgvQLSV.Rows[index].Cells[1].Value = s.FullName;
                dgvQLSV.Rows[index].Cells[2].Value = s.Faculty?.FacultyName;
                dgvQLSV.Rows[index].Cells[3].Value = s.AverageScore;
                dgvQLSV.Rows[index].Cells[4].Value = s.Major?.Name ?? "(Chưa có)";
            }
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkChuaDK.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = dlg.FileName;
                    picAnh.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        private void btnThemSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMSSV.Text))
                {
                    MessageBox.Show("Vui lòng nhập Mã số sinh viên!");
                    return;
                }

                var s = new Student
                {
                    StudentID = txtMSSV.Text.Trim(),
                    FullName = txtHT.Text.Trim(),
                    FacultyID = (int)cmbKhoa.SelectedValue,
                    AverageScore = double.TryParse(txtDTB.Text, out double d) ? d : 0
                };

                // Xử lý ảnh
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    string ext = Path.GetExtension(selectedImagePath);
                    string fileName = $"{s.StudentID}{ext}";
                    string baseDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string imageDir = Path.Combine(baseDir, "Images");
                    if (!Directory.Exists(imageDir))
                        Directory.CreateDirectory(imageDir);
                    string dest = Path.Combine(imageDir, fileName);
                    File.Copy(selectedImagePath, dest, true);
                    s.Avatar = fileName;
                }

                studentService.InsertUpdate(s);
                MessageBox.Show("Lưu sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStudentList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string id = txtMSSV.Text.Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                studentService.Delete(id);
                MessageBox.Show("Đã xóa sinh viên!", "Thông báo");
                LoadStudentList();
            }
        }

        private void ShowAvatar(string imageName)
        {
            try
            {
                if (string.IsNullOrEmpty(imageName))
                {
                    picAnh.Image = null;
                    return;
                }

                string baseDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string path = Path.Combine(baseDir, "Images", imageName);
                picAnh.Image = File.Exists(path) ? Image.FromFile(path) : null;
            }
            catch
            {
                picAnh.Image = null;
            }
        }

        private void chkChuaDK_CheckedChanged(object sender, EventArgs e)
        {
            var list = chkChuaDK.Checked
                ? studentService.GetAllHasNoMajor()
                : studentService.GetAll();
            BindGrid(list);
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    btnThemSua_Click(sender, e);
        //}

        private void dgvQLSV_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQLSV.SelectedRows.Count > 0)
            {
                string id = dgvQLSV.SelectedRows[0].Cells[0].Value?.ToString();
                var s = studentService.FindById(id);
                if (s != null)
                {
                    txtMSSV.Text = s.StudentID;
                    txtHT.Text = s.FullName;
                    txtDTB.Text = s.AverageScore.ToString();
                    cmbKhoa.SelectedValue = s.FacultyID ?? 0;
                    ShowAvatar(s.Avatar);
                }
            }
        }
    }
}

    

