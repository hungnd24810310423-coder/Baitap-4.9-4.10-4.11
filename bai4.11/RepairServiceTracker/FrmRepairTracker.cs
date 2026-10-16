using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace RepairServiceTracker;

public class FrmRepairTracker : Form
{
    private readonly BindingList<RepairOrder> _orders = new();
    private readonly BindingSource _source = new();
    private RepairOrder? _selectedOrder;
    private string _selectedImagePath = "";

    private TextBox txtOrderId = new(), txtCustomerName = new(), txtPhone = new(),
        txtDescription = new(), txtCost = new(), txtSearch = new();
    private ComboBox cboDeviceType = new(), cboStatus = new(), cboFilterStatus = new();
    private DateTimePicker dtpReceivedDate = new();
    private PictureBox picDevice = new();
    private DataGridView dgvOrders = new();
    private ErrorProvider errorProvider = new();
    private ToolStripStatusLabel lblStatus = new();

    private readonly string[] _statuses =
        { "Mới tiếp nhận", "Đang sửa", "Chờ linh kiện", "Hoàn tất", "Đã hủy" };

    public FrmRepairTracker()
    {
        InitializeComponent();
        InitializeData();
        RefreshList();
    }

    private void InitializeData()
    {
        cboDeviceType.Items.AddRange(new object[] { "Điện thoại", "Laptop", "Máy tính bàn", "Máy tính bảng", "Thiết bị khác" });
        cboStatus.Items.AddRange(_statuses);
        cboFilterStatus.Items.AddRange(new object[] { "Tất cả trạng thái" }.Concat(_statuses).ToArray());
        cboDeviceType.SelectedIndex = 0;
        cboStatus.SelectedIndex = 0;
        cboFilterStatus.SelectedIndex = 0;
        _source.DataSource = new BindingList<RepairOrder>();
        dgvOrders.DataSource = _source;
    }

    private void InitializeComponent()
    {
        Text = "Repair Service Tracker - Quản lý đơn sửa chữa";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);
        ClientSize = new Size(1400, 820);
        Font = new Font("Segoe UI", 9F);
        errorProvider.ContainerControl = this;

        var menu = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("Tệp");
        var export = new ToolStripMenuItem("Xuất danh sách đang hiển thị ra CSV");
        var exit = new ToolStripMenuItem("Thoát");
        export.Click += ExportCsv_Click;
        exit.Click += (_, _) => Close();
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { export, new ToolStripSeparator(), exit });
        menu.Items.Add(fileMenu);
        MainMenuStrip = menu;

        var statusStrip = new StatusStrip();
        lblStatus.Text = "Số đơn: 0 | Tổng chi phí chưa hoàn tất: 0";
        statusStrip.Items.Add(lblStatus);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(8) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var left = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(8) };
        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        layout.Controls.Add(left, 0, 0);
        layout.Controls.Add(right, 1, 0);

        var heading = new Label { Text = "THÔNG TIN ĐƠN SỬA CHỮA", Font = new Font("Segoe UI", 13, FontStyle.Bold), AutoSize = true, Location = new Point(8, 8) };
        left.Controls.Add(heading);
        int y = 48;
        AddField(left, "Mã đơn *", txtOrderId, ref y, 30);
        AddField(left, "Tên khách hàng *", txtCustomerName, ref y, 30);
        AddField(left, "Số điện thoại *", txtPhone, ref y, 30);
        AddCombo(left, "Loại thiết bị", cboDeviceType, ref y);
        AddField(left, "Mô tả lỗi *", txtDescription, ref y, 68);
        txtDescription.Multiline = true;
        txtDescription.ScrollBars = ScrollBars.Vertical;
        AddDate(left, "Ngày tiếp nhận", dtpReceivedDate, ref y);
        AddField(left, "Chi phí dự kiến *", txtCost, ref y, 30);
        AddCombo(left, "Trạng thái", cboStatus, ref y);

        var lblImage = new Label { Text = "Ảnh thiết bị (không bắt buộc)", AutoSize = true, Location = new Point(12, y + 2) };
        left.Controls.Add(lblImage);
        y += 24;
        picDevice.Location = new Point(12, y);
        picDevice.Size = new Size(390, 125);
        picDevice.BorderStyle = BorderStyle.FixedSingle;
        picDevice.SizeMode = PictureBoxSizeMode.Zoom;
        left.Controls.Add(picDevice);
        y += 133;
        var btnChooseImage = new Button { Text = "Chọn ảnh...", Location = new Point(12, y), Size = new Size(100, 32) };
        btnChooseImage.Click += ChooseImage_Click;
        left.Controls.Add(btnChooseImage);
        y += 45;

        var buttons = new FlowLayoutPanel { Location = new Point(8, y), Size = new Size(405, 42), WrapContents = true, AutoSize = false };
        var btnAdd = new Button { Text = "Thêm mới", Width = 95, Height = 32 };
        var btnUpdate = new Button { Text = "Cập nhật", Width = 95, Height = 32 };
        var btnDelete = new Button { Text = "Xóa", Width = 75, Height = 32 };
        var btnClear = new Button { Text = "Làm mới", Width = 85, Height = 32 };
        btnAdd.Click += Add_Click;
        btnUpdate.Click += Update_Click;
        btnDelete.Click += Delete_Click;
        btnClear.Click += (_, _) => ClearForm();
        buttons.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnClear });
        left.Controls.Add(buttons);

        var lblSearch = new Label { Text = "Tìm theo mã đơn / tên / điện thoại:", AutoSize = true, Location = new Point(8, 12) };
        txtSearch.Location = new Point(8, 35);
        txtSearch.Width = 360;
        txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtSearch.TextChanged += (_, _) => RefreshList();
        var lblFilter = new Label { Text = "Lọc trạng thái:", AutoSize = true, Location = new Point(385, 12) };
        cboFilterStatus.Location = new Point(385, 34);
        cboFilterStatus.Width = 200;
        cboFilterStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterStatus.SelectedIndexChanged += (_, _) => RefreshList();
        right.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblFilter, cboFilterStatus });

        dgvOrders.Location = new Point(8, 75);
        dgvOrders.Size = new Size(900, 650);
        dgvOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvOrders.AutoGenerateColumns = false;
        dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrders.MultiSelect = false;
        dgvOrders.ReadOnly = true;
        dgvOrders.AllowUserToAddRows = false;
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOrders.RowHeadersVisible = false;
        dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;
        AddGridColumn("Mã đơn", nameof(RepairOrder.OrderId));
        AddGridColumn("Khách hàng", nameof(RepairOrder.CustomerName));
        AddGridColumn("Điện thoại", nameof(RepairOrder.Phone));
        AddGridColumn("Thiết bị", nameof(RepairOrder.DeviceType));
        AddGridColumn("Ngày nhận", nameof(RepairOrder.ReceivedDate), "dd/MM/yyyy");
        AddGridColumn("Chi phí dự kiến", nameof(RepairOrder.EstimatedCost), "N0");
        AddGridColumn("Trạng thái", nameof(RepairOrder.Status));
        right.Controls.Add(dgvOrders);

        Controls.Add(layout);
        Controls.Add(statusStrip);
        Controls.Add(menu);
        menu.Dock = DockStyle.Top;
        statusStrip.Dock = DockStyle.Bottom;
    }

    private void AddGridColumn(string header, string property, string? format = null)
    {
        var col = new DataGridViewTextBoxColumn { HeaderText = header, DataPropertyName = property, SortMode = DataGridViewColumnSortMode.Automatic };
        if (format != null) col.DefaultCellStyle.Format = format;
        dgvOrders.Columns.Add(col);
    }

    private static void AddField(Control parent, string label, TextBox box, ref int y, int height)
    {
        parent.Controls.Add(new Label { Text = label, AutoSize = true, Location = new Point(12, y + 4) });
        box.Location = new Point(145, y);
        box.Width = 255;
        box.Height = height;
        box.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        parent.Controls.Add(box);
        y += height == 30 ? 37 : 76;
    }

    private static void AddCombo(Control parent, string label, ComboBox combo, ref int y)
    {
        parent.Controls.Add(new Label { Text = label, AutoSize = true, Location = new Point(12, y + 4) });
        combo.Location = new Point(145, y);
        combo.Width = 255;
        combo.DropDownStyle = ComboBoxStyle.DropDownList;
        parent.Controls.Add(combo);
        y += 37;
    }

    private static void AddDate(Control parent, string label, DateTimePicker picker, ref int y)
    {
        parent.Controls.Add(new Label { Text = label, AutoSize = true, Location = new Point(12, y + 4) });
        picker.Location = new Point(145, y);
        picker.Width = 255;
        picker.Format = DateTimePickerFormat.Custom;
        picker.CustomFormat = "dd/MM/yyyy";
        parent.Controls.Add(picker);
        y += 37;
    }

    private bool ValidateForm()
    {
        errorProvider.Clear();
        bool valid = true;
        string id = txtOrderId.Text.Trim();

        if (id.Length == 0) { errorProvider.SetError(txtOrderId, "Mã đơn không được để trống."); valid = false; }
        else if (_orders.Any(o => o.OrderId.Equals(id, StringComparison.OrdinalIgnoreCase) && !ReferenceEquals(o, _selectedOrder)))
        { errorProvider.SetError(txtOrderId, "Mã đơn đã tồn tại."); valid = false; }

        if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
        { errorProvider.SetError(txtCustomerName, "Tên khách hàng không được để trống."); valid = false; }
        if (string.IsNullOrWhiteSpace(txtPhone.Text))
        { errorProvider.SetError(txtPhone, "Số điện thoại không được để trống."); valid = false; }
        if (string.IsNullOrWhiteSpace(txtDescription.Text))
        { errorProvider.SetError(txtDescription, "Mô tả lỗi là bắt buộc."); valid = false; }
        if (!decimal.TryParse(txtCost.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal cost) || cost < 0)
        { errorProvider.SetError(txtCost, "Chi phí phải là số không âm."); valid = false; }
        return valid;
    }

    private RepairOrder ReadForm() => new()
    {
        OrderId = txtOrderId.Text.Trim(),
        CustomerName = txtCustomerName.Text.Trim(),
        Phone = txtPhone.Text.Trim(),
        DeviceType = cboDeviceType.Text,
        ProblemDescription = txtDescription.Text.Trim(),
        ReceivedDate = dtpReceivedDate.Value.Date,
        EstimatedCost = decimal.Parse(txtCost.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture),
        Status = cboStatus.Text,
        ImagePath = _selectedImagePath
    };

    private void Add_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm()) return;
        _orders.Add(ReadForm());
        RefreshList();
        ClearForm();
    }

    private void Update_Click(object? sender, EventArgs e)
    {
        if (_selectedOrder == null)
        {
            MessageBox.Show("Vui lòng chọn đơn cần cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!ValidateForm()) return;
        var value = ReadForm();
        _selectedOrder.OrderId = value.OrderId;
        _selectedOrder.CustomerName = value.CustomerName;
        _selectedOrder.Phone = value.Phone;
        _selectedOrder.DeviceType = value.DeviceType;
        _selectedOrder.ProblemDescription = value.ProblemDescription;
        _selectedOrder.ReceivedDate = value.ReceivedDate;
        _selectedOrder.EstimatedCost = value.EstimatedCost;
        _selectedOrder.Status = value.Status;
        _selectedOrder.ImagePath = value.ImagePath;
        RefreshList();
    }

    private void Delete_Click(object? sender, EventArgs e)
    {
        if (_selectedOrder == null)
        {
            MessageBox.Show("Vui lòng chọn đơn cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"Bạn chắc chắn muốn xóa đơn {_selectedOrder.OrderId}?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        _orders.Remove(_selectedOrder);
        ClearForm();
        RefreshList();
    }

    private void ChooseImage_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Chọn ảnh thiết bị",
            Filter = "Tệp ảnh|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            using var image = Image.FromFile(dialog.FileName);
            picDevice.Image?.Dispose();
            picDevice.Image = new Bitmap(image);
            _selectedImagePath = dialog.FileName;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không thể mở ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DgvOrders_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvOrders.CurrentRow?.DataBoundItem is not RepairOrder order) return;
        _selectedOrder = order;
        txtOrderId.Text = order.OrderId;
        txtCustomerName.Text = order.CustomerName;
        txtPhone.Text = order.Phone;
        cboDeviceType.Text = order.DeviceType;
        txtDescription.Text = order.ProblemDescription;
        dtpReceivedDate.Value = order.ReceivedDate;
        txtCost.Text = order.EstimatedCost.ToString("0.##", CultureInfo.CurrentCulture);
        cboStatus.Text = order.Status;
        _selectedImagePath = order.ImagePath;
        picDevice.Image?.Dispose();
        picDevice.Image = null;
        if (!string.IsNullOrWhiteSpace(order.ImagePath) && File.Exists(order.ImagePath))
        {
            using var image = Image.FromFile(order.ImagePath);
            picDevice.Image = new Bitmap(image);
        }
        errorProvider.Clear();
    }

    private void RefreshList()
    {
        string keyword = txtSearch.Text.Trim();
        string status = cboFilterStatus.SelectedItem?.ToString() ?? "Tất cả trạng thái";
        var visible = _orders.Where(o =>
            (keyword.Length == 0 ||
             o.OrderId.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
             o.CustomerName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
             o.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase)) &&
            (status == "Tất cả trạng thái" || o.Status == status)).ToList();

        _source.DataSource = new BindingList<RepairOrder>(visible);
        dgvOrders.DataSource = _source;
        decimal pendingCost = _orders.Where(o => o.Status != "Hoàn tất").Sum(o => o.EstimatedCost);
        lblStatus.Text = $"Số đơn đang hiển thị: {visible.Count} | Tổng chi phí dự kiến chưa hoàn tất: {pendingCost:N0} VNĐ";
    }

    private void ExportCsv_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "DanhSachDonSuaChua.csv", Title = "Xuất danh sách đang hiển thị" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        var visible = (_source.DataSource as BindingList<RepairOrder>)?.ToList() ?? new List<RepairOrder>();
        var sb = new StringBuilder();
        sb.AppendLine("MaDon,TenKhachHang,SoDienThoai,LoaiThietBi,MoTaLoi,NgayTiepNhan,ChiPhiDuKien,TrangThai,AnhThietBi");
        foreach (var o in visible)
        {
            string[] fields = { o.OrderId, o.CustomerName, o.Phone, o.DeviceType, o.ProblemDescription,
                o.ReceivedDate.ToString("dd/MM/yyyy"), o.EstimatedCost.ToString(CultureInfo.InvariantCulture), o.Status, o.ImagePath };
            sb.AppendLine(string.Join(",", fields.Select(CsvEscape)));
        }
        File.WriteAllText(dialog.FileName, "\uFEFF" + sb.ToString(), Encoding.UTF8);
        MessageBox.Show("Đã xuất danh sách đang hiển thị ra CSV.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static string CsvEscape(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";

    private void ClearForm()
    {
        _selectedOrder = null;
        txtOrderId.Clear();
        txtCustomerName.Clear();
        txtPhone.Clear();
        txtDescription.Clear();
        txtCost.Clear();
        cboDeviceType.SelectedIndex = 0;
        cboStatus.SelectedIndex = 0;
        dtpReceivedDate.Value = DateTime.Today;
        _selectedImagePath = "";
        picDevice.Image?.Dispose();
        picDevice.Image = null;
        dgvOrders.ClearSelection();
        errorProvider.Clear();
    }
}
