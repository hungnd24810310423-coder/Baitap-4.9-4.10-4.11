using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace ClinicAppointmentManager;

public partial class FrmAppointmentManager : Form
{
    private readonly BindingList<Appointment> _appointments = new();
    private readonly BindingSource _source = new();
    private Appointment? _selectedAppointment;
    private bool _loadingRow;

    public FrmAppointmentManager()
    {
        InitializeComponent();
        InitializeData();
    }

    private void InitializeData()
    {
        cboDoctor.Items.AddRange(new object[] { "BS. Nguyễn Văn An", "BS. Trần Thị Bình", "BS. Lê Minh Châu" });
        cboStatus.Items.AddRange(new object[] { "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy" });
        cboStatus.SelectedIndex = 0;
        dtpAppointment.Format = DateTimePickerFormat.Custom;
        dtpAppointment.CustomFormat = "dd/MM/yyyy HH:mm";
        dtpAppointment.ShowUpDown = true;
        dtpFilterDate.Format = DateTimePickerFormat.Custom;
        dtpFilterDate.CustomFormat = "dd/MM/yyyy";
        dtpFilterDate.ShowCheckBox = true;
        dtpFilterDate.Checked = false;

        dgvAppointments.AutoGenerateColumns = false;
        dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAppointments.MultiSelect = false;
        dgvAppointments.ReadOnly = true;
        dgvAppointments.AllowUserToAddRows = false;
        dgvAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Appointment.AppointmentId), HeaderText = "Mã lịch", Width = 95 });
        dgvAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Appointment.PatientName), HeaderText = "Bệnh nhân", Width = 150 });
        dgvAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Appointment.Phone), HeaderText = "Điện thoại", Width = 110 });
        dgvAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Appointment.AppointmentDate), HeaderText = "Ngày giờ hẹn", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
        dgvAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Appointment.Doctor), HeaderText = "Bác sĩ", Width = 150 });
        dgvAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Appointment.Reason), HeaderText = "Lý do khám", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 130 });
        dgvAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Appointment.Status), HeaderText = "Trạng thái", Width = 100 });

        _source.DataSource = _appointments;
        dgvAppointments.DataSource = _source;
        UpdateStatus();
    }

    private bool ValidateInput()
    {
        errorProvider.Clear();
        bool valid = true;
        string id = txtAppointmentId.Text.Trim();
        string name = txtPatientName.Text.Trim();
        string phone = txtPhone.Text.Trim();
        string reason = txtReason.Text.Trim();

        if (id.Length == 0)
        {
            errorProvider.SetError(txtAppointmentId, "Vui lòng nhập mã lịch hẹn.");
            valid = false;
        }
        else if (_appointments.Any(a => a.AppointmentId.Equals(id, StringComparison.OrdinalIgnoreCase) && !ReferenceEquals(a, _selectedAppointment)))
        {
            errorProvider.SetError(txtAppointmentId, "Mã lịch hẹn đã tồn tại.");
            valid = false;
        }

        if (name.Length == 0)
        {
            errorProvider.SetError(txtPatientName, "Vui lòng nhập họ tên bệnh nhân.");
            valid = false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9}$"))
        {
            errorProvider.SetError(txtPhone, "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0.");
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(cboDoctor.Text))
        {
            errorProvider.SetError(cboDoctor, "Vui lòng chọn bác sĩ.");
            valid = false;
        }

        if (reason.Length == 0)
        {
            errorProvider.SetError(txtReason, "Vui lòng nhập lý do khám.");
            valid = false;
        }

        if (dtpAppointment.Value < DateTime.Now)
        {
            errorProvider.SetError(dtpAppointment, "Ngày giờ hẹn không được nằm trong quá khứ.");
            valid = false;
        }
        return valid;
    }

    private Appointment ReadForm() => new()
    {
        AppointmentId = txtAppointmentId.Text.Trim(),
        PatientName = txtPatientName.Text.Trim(),
        Phone = txtPhone.Text.Trim(),
        AppointmentDate = dtpAppointment.Value,
        Doctor = cboDoctor.Text,
        Reason = txtReason.Text.Trim(),
        Status = cboStatus.Text
    };

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        _appointments.Add(ReadForm());
        ApplyFilters();
        ClearForm();
    }

    private void btnUpdate_Click(object? sender, EventArgs e)
    {
        if (_selectedAppointment is null)
        {
            MessageBox.Show("Hãy chọn lịch hẹn cần cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!ValidateInput()) return;
        var updated = ReadForm();
        _selectedAppointment.AppointmentId = updated.AppointmentId;
        _selectedAppointment.PatientName = updated.PatientName;
        _selectedAppointment.Phone = updated.Phone;
        _selectedAppointment.AppointmentDate = updated.AppointmentDate;
        _selectedAppointment.Doctor = updated.Doctor;
        _selectedAppointment.Reason = updated.Reason;
        _selectedAppointment.Status = updated.Status;
        _source.ResetBindings(false);
        ApplyFilters();
        UpdateStatus();
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedAppointment is null)
        {
            MessageBox.Show("Hãy chọn lịch hẹn cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"Bạn có chắc muốn xóa lịch hẹn của {_selectedAppointment.PatientName}?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        _appointments.Remove(_selectedAppointment);
        _selectedAppointment = null;
        ClearForm();
        ApplyFilters();
    }

    private void dgvAppointments_SelectionChanged(object? sender, EventArgs e)
    {
        if (_loadingRow || dgvAppointments.CurrentRow?.DataBoundItem is not Appointment a) return;
        _selectedAppointment = a;
        _loadingRow = true;
        txtAppointmentId.Text = a.AppointmentId;
        txtPatientName.Text = a.PatientName;
        txtPhone.Text = a.Phone;
        dtpAppointment.Value = a.AppointmentDate;
        cboDoctor.Text = a.Doctor;
        txtReason.Text = a.Reason;
        cboStatus.Text = a.Status;
        errorProvider.Clear();
        _loadingRow = false;
    }

    private void txtSearch_TextChanged(object? sender, EventArgs e) => ApplyFilters();
    private void filterChanged(object? sender, EventArgs e) => ApplyFilters();

    private void ApplyFilters()
    {
        string keyword = txtSearch.Text.Trim();
        string status = cboFilterStatus.SelectedItem?.ToString() ?? "Tất cả";
        DateTime date = dtpFilterDate.Value.Date;
        var filtered = _appointments.Where(a =>
            (keyword.Length == 0 ||
             a.PatientName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
             a.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase)) &&
            (status == "Tất cả" || a.Status == status) &&
            (!dtpFilterDate.Checked || a.AppointmentDate.Date == date)).ToList();

        _source.DataSource = new BindingList<Appointment>(filtered);
        dgvAppointments.DataSource = _source;
        UpdateStatus(filtered.Count);
    }

    private void UpdateStatus(int? count = null)
    {
        int total = count ?? _appointments.Count;
        toolStatusLabel.Text = $"Số lịch hẹn đang hiển thị: {total}";
    }

    private void ClearForm()
    {
        _selectedAppointment = null;
        txtAppointmentId.Clear();
        txtPatientName.Clear();
        txtPhone.Clear();
        txtReason.Clear();
        if (cboDoctor.Items.Count > 0) cboDoctor.SelectedIndex = 0;
        cboStatus.SelectedItem = "Chờ khám";
        dtpAppointment.Value = DateTime.Now.AddMinutes(30);
        errorProvider.Clear();
        dgvAppointments.ClearSelection();
    }

    private void exportCsvToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog { Filter = "CSV file|*.csv", FileName = "lich-hen.csv", Title = "Xuất danh sách lịch hẹn" };
        if (dialog.ShowDialog() != DialogResult.OK) return;
        var visible = _source.List.Cast<Appointment>().ToList();
        var sb = new StringBuilder();
        sb.AppendLine("MaLich,HoTen,SoDienThoai,NgayGio,BacSi,LyDoKham,TrangThai");
        foreach (var a in visible)
        {
            string Csv(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";
            sb.AppendLine(string.Join(",", Csv(a.AppointmentId), Csv(a.PatientName), Csv(a.Phone),
                Csv(a.AppointmentDate.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)),
                Csv(a.Doctor), Csv(a.Reason), Csv(a.Status)));
        }
        File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
        MessageBox.Show("Xuất danh sách đang hiển thị ra CSV thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void exitToolStripMenuItem_Click(object? sender, EventArgs e) => Close();
}
