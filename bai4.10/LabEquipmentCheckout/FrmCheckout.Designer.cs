namespace LabEquipmentCheckout;

partial class FrmCheckout
{
    private System.ComponentModel.IContainer? components = null;
    private TextBox txtReceiptId = null!, txtStudentId = null!, txtStudentName = null!, txtSearch = null!;
    private ComboBox cboEquipment = null!, cboFilter = null!;
    private NumericUpDown nudQuantity = null!;
    private DateTimePicker dtpBorrow = null!, dtpDue = null!, dtpReturn = null!;
    private DataGridView dgvRecords = null!;
    private Button btnAdd = null!, btnUpdate = null!, btnDelete = null!, btnMarkReturned = null!, btnSearch = null!, btnClearSearch = null!, btnExport = null!, btnClear = null!;
    private ErrorProvider errorProvider = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel toolStripStatusLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        txtReceiptId = new TextBox(); txtStudentId = new TextBox(); txtStudentName = new TextBox(); txtSearch = new TextBox();
        cboEquipment = new ComboBox(); cboFilter = new ComboBox(); nudQuantity = new NumericUpDown();
        dtpBorrow = new DateTimePicker(); dtpDue = new DateTimePicker(); dtpReturn = new DateTimePicker { ShowCheckBox = true };
        dgvRecords = new DataGridView();
        btnAdd = new Button(); btnUpdate = new Button(); btnDelete = new Button(); btnMarkReturned = new Button();
        btnSearch = new Button(); btnClearSearch = new Button(); btnExport = new Button(); btnClear = new Button();
        errorProvider = new ErrorProvider(components); statusStrip = new StatusStrip(); toolStripStatusLabel = new ToolStripStatusLabel();
        var main = new TableLayoutPanel(); var formGroup = new GroupBox(); var form = new TableLayoutPanel();
        var searchPanel = new FlowLayoutPanel(); var gridGroup = new GroupBox();
        ((System.ComponentModel.ISupportInitialize)nudQuantity).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvRecords).BeginInit();
        SuspendLayout();

        Text = "Lab Equipment Checkout - Quản lý mượn trả thiết bị";
        StartPosition = FormStartPosition.CenterScreen; MinimumSize = new Size(1050, 650);
        Size = new Size(1350, 820); Font = new Font("Segoe UI", 9F);
        main.Dock = DockStyle.Fill; main.Padding = new Padding(10); main.ColumnCount = 1; main.RowCount = 2;
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 255F)); main.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        formGroup.Text = "Thông tin phiếu mượn"; formGroup.Dock = DockStyle.Fill; formGroup.Padding = new Padding(10);
        form.Dock = DockStyle.Fill; form.ColumnCount = 6; form.RowCount = 4;
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        for (int i = 0; i < 4; i++) form.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        AddField(form, "Mã phiếu:", txtReceiptId, 0, 0);
        AddField(form, "Mã sinh viên:", txtStudentId, 2, 0);
        AddField(form, "Họ tên:", txtStudentName, 4, 0);
        AddField(form, "Thiết bị:", cboEquipment, 0, 1);
        AddField(form, "Số lượng:", nudQuantity, 2, 1);
        AddField(form, "Ngày mượn:", dtpBorrow, 4, 1);
        AddField(form, "Hạn trả:", dtpDue, 0, 2);
        AddField(form, "Ngày trả TT:", dtpReturn, 2, 2);
        var note = new Label { Text = "Bỏ chọn ô ngày trả nếu chưa trả thiết bị.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DimGray };
        form.Controls.Add(note, 4, 2); form.SetColumnSpan(note, 2);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
        ConfigureButton(btnAdd, "Thêm"); btnAdd.Click += btnAdd_Click;
        ConfigureButton(btnUpdate, "Cập nhật"); btnUpdate.Click += btnUpdate_Click;
        ConfigureButton(btnDelete, "Xóa"); btnDelete.Click += btnDelete_Click;
        ConfigureButton(btnMarkReturned, "Đánh dấu đã trả"); btnMarkReturned.Click += btnMarkReturned_Click;
        ConfigureButton(btnClear, "Làm mới"); btnClear.Click += (s, e) => { ClearInput(); dgvRecords.ClearSelection(); };
        buttons.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnMarkReturned, btnClear });
        form.Controls.Add(buttons, 0, 3); form.SetColumnSpan(buttons, 6); formGroup.Controls.Add(form);

        gridGroup.Text = "Danh sách phiếu mượn"; gridGroup.Dock = DockStyle.Fill; gridGroup.Padding = new Padding(8);
        var gridLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        gridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42)); gridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        searchPanel.Dock = DockStyle.Fill; searchPanel.WrapContents = false; searchPanel.Padding = new Padding(0, 4, 0, 0);
        searchPanel.Controls.Add(new Label { Text = "Tìm mã SV / thiết bị:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        txtSearch.Width = 220; txtSearch.PlaceholderText = "Nhập từ khóa...";
        ConfigureButton(btnSearch, "Tìm"); btnSearch.Click += btnSearch_Click;
        ConfigureButton(btnClearSearch, "Bỏ tìm"); btnClearSearch.Click += btnClearSearch_Click;
        searchPanel.Controls.Add(txtSearch); searchPanel.Controls.Add(btnSearch); searchPanel.Controls.Add(btnClearSearch);
        searchPanel.Controls.Add(new Label { Text = "Lọc trạng thái:", AutoSize = true, Padding = new Padding(8, 7, 0, 0) });
        cboFilter.DropDownStyle = ComboBoxStyle.DropDownList; cboFilter.Width = 150;
        cboFilter.Items.AddRange(new object[] { "Tất cả", "Đang mượn", "Quá hạn", "Đã trả" });
        cboFilter.SelectedIndexChanged += cboFilter_SelectedIndexChanged; searchPanel.Controls.Add(cboFilter);
        ConfigureButton(btnExport, "Xuất CSV"); btnExport.Click += btnExport_Click; searchPanel.Controls.Add(btnExport);
        dgvRecords.Dock = DockStyle.Fill; dgvRecords.AllowUserToAddRows = false; dgvRecords.AllowUserToDeleteRows = false;
        dgvRecords.ReadOnly = true; dgvRecords.MultiSelect = false; dgvRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; dgvRecords.RowHeadersVisible = false;
        dgvRecords.BackgroundColor = Color.White; dgvRecords.SelectionChanged += dgvRecords_SelectionChanged;
        AddColumn("MaPhieu", "Mã phiếu"); AddColumn("MaSinhVien", "Mã SV"); AddColumn("HoTen", "Họ tên");
        AddColumn("ThietBi", "Thiết bị"); AddColumn("SoLuong", "SL"); AddColumn("NgayMuon", "Ngày mượn", "dd/MM/yyyy");
        AddColumn("HanTra", "Hạn trả", "dd/MM/yyyy"); AddColumn("NgayTraThucTe", "Ngày trả TT", "dd/MM/yyyy");
        AddColumn("TinhTrang", "Tình trạng");
        gridLayout.Controls.Add(searchPanel, 0, 0); gridLayout.Controls.Add(dgvRecords, 0, 1); gridGroup.Controls.Add(gridLayout);

        main.Controls.Add(formGroup, 0, 0); main.Controls.Add(gridGroup, 0, 1);
        statusStrip.Items.Add(toolStripStatusLabel); statusStrip.Dock = DockStyle.Bottom;
        Controls.Add(main); Controls.Add(statusStrip);
        errorProvider.ContainerControl = this; errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        ((System.ComponentModel.ISupportInitialize)nudQuantity).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvRecords).EndInit();
        ResumeLayout(false); PerformLayout();
    }

    private static void AddField(TableLayoutPanel panel, string text, Control control, int col, int row)
    {
        var label = new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoSize = false };
        control.Dock = DockStyle.Fill; control.Margin = new Padding(3, 5, 8, 5);
        panel.Controls.Add(label, col, row); panel.Controls.Add(control, col + 1, row);
    }

    private static void ConfigureButton(Button button, string text)
    {
        button.Text = text; button.AutoSize = true; button.Height = 30; button.Margin = new Padding(4);
        button.Padding = new Padding(5, 0, 5, 0);
    }

    private void AddColumn(string property, string header, string? format = null)
    {
        var column = new DataGridViewTextBoxColumn
        {
            DataPropertyName = property, HeaderText = header, Name = property,
            SortMode = DataGridViewColumnSortMode.Automatic
        };
        if (format != null) column.DefaultCellStyle.Format = format;
        dgvRecords.Columns.Add(column);
    }
}
