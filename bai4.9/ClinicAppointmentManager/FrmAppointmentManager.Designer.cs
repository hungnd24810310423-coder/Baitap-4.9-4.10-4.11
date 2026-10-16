namespace ClinicAppointmentManager;

partial class FrmAppointmentManager
{
    private System.ComponentModel.IContainer? components = null;
    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem fileMenu = null!;
    private ToolStripMenuItem exportCsvMenuItem = null!;
    private ToolStripMenuItem exitMenuItem = null!;
    private TableLayoutPanel mainLayout = null!;
    private TableLayoutPanel inputLayout = null!;
    private TableLayoutPanel filterLayout = null!;
    private DataGridView dgvAppointments = null!;
    private TextBox txtAppointmentId = null!;
    private TextBox txtPatientName = null!;
    private TextBox txtPhone = null!;
    private TextBox txtReason = null!;
    private ComboBox cboDoctor = null!;
    private ComboBox cboStatus = null!;
    private DateTimePicker dtpAppointment = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboFilterStatus = null!;
    private DateTimePicker dtpFilterDate = null!;
    private Button btnAdd = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private Button btnClear = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel toolStatusLabel = null!;
    private ErrorProvider errorProvider = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        menuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem();
        exportCsvMenuItem = new ToolStripMenuItem();
        exitMenuItem = new ToolStripMenuItem();
        mainLayout = new TableLayoutPanel();
        inputLayout = new TableLayoutPanel();
        filterLayout = new TableLayoutPanel();
        dgvAppointments = new DataGridView();
        txtAppointmentId = new TextBox();
        txtPatientName = new TextBox();
        txtPhone = new TextBox();
        txtReason = new TextBox();
        cboDoctor = new ComboBox();
        cboStatus = new ComboBox();
        dtpAppointment = new DateTimePicker();
        txtSearch = new TextBox();
        cboFilterStatus = new ComboBox();
        dtpFilterDate = new DateTimePicker();
        btnAdd = new Button();
        btnUpdate = new Button();
        btnDelete = new Button();
        btnClear = new Button();
        statusStrip = new StatusStrip();
        toolStatusLabel = new ToolStripStatusLabel();
        errorProvider = new ErrorProvider(components);
        var lblTitle = new Label();
        var lblId = new Label();
        var lblName = new Label();
        var lblPhone = new Label();
        var lblDate = new Label();
        var lblDoctor = new Label();
        var lblReason = new Label();
        var lblStatus = new Label();
        var lblSearch = new Label();
        var lblFilterDate = new Label();
        var lblFilterStatus = new Label();
        ((System.ComponentModel.ISupportInitialize)dgvAppointments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
        SuspendLayout();

        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1100, 24);
        fileMenu.Text = "Tệp";
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { exportCsvMenuItem, exitMenuItem });
        exportCsvMenuItem.Text = "Xuất danh sách đang hiển thị ra CSV...";
        exportCsvMenuItem.Click += exportCsvToolStripMenuItem_Click;
        exitMenuItem.Text = "Thoát";
        exitMenuItem.Click += exitToolStripMenuItem_Click;

        mainLayout.Dock = DockStyle.Fill;
        mainLayout.Padding = new Padding(12);
        mainLayout.ColumnCount = 1;
        mainLayout.RowCount = 4;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 205));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        lblTitle.Text = "QUẢN LÝ LỊCH HẸN PHÒNG KHÁM";
        lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.Height = 38;

        inputLayout.Dock = DockStyle.Fill;
        inputLayout.ColumnCount = 4;
        inputLayout.RowCount = 4;
        inputLayout.Padding = new Padding(4);
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 4; i++) inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        SetupLabel(lblId, "Mã lịch hẹn");
        SetupLabel(lblName, "Họ tên bệnh nhân");
        SetupLabel(lblPhone, "Số điện thoại");
        SetupLabel(lblDate, "Ngày giờ hẹn");
        SetupLabel(lblDoctor, "Bác sĩ");
        SetupLabel(lblReason, "Lý do khám");
        SetupLabel(lblStatus, "Trạng thái");
        txtAppointmentId.Dock = DockStyle.Fill;
        txtPatientName.Dock = DockStyle.Fill;
        txtPhone.Dock = DockStyle.Fill;
        txtReason.Dock = DockStyle.Fill;
        cboDoctor.Dock = DockStyle.Fill;
        cboDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatus.Dock = DockStyle.Fill;
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        dtpAppointment.Dock = DockStyle.Fill;

        inputLayout.Controls.Add(lblId, 0, 0); inputLayout.Controls.Add(txtAppointmentId, 1, 0);
        inputLayout.Controls.Add(lblName, 2, 0); inputLayout.Controls.Add(txtPatientName, 3, 0);
        inputLayout.Controls.Add(lblPhone, 0, 1); inputLayout.Controls.Add(txtPhone, 1, 1);
        inputLayout.Controls.Add(lblDate, 2, 1); inputLayout.Controls.Add(dtpAppointment, 3, 1);
        inputLayout.Controls.Add(lblDoctor, 0, 2); inputLayout.Controls.Add(cboDoctor, 1, 2);
        inputLayout.Controls.Add(lblStatus, 2, 2); inputLayout.Controls.Add(cboStatus, 3, 2);
        inputLayout.Controls.Add(lblReason, 0, 3); inputLayout.Controls.Add(txtReason, 1, 3); inputLayout.SetColumnSpan(txtReason, 3);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Padding = new Padding(3) };
        btnAdd.Text = "Thêm"; btnUpdate.Text = "Cập nhật"; btnDelete.Text = "Xóa"; btnClear.Text = "Làm mới";
        foreach (var b in new[] { btnAdd, btnUpdate, btnDelete, btnClear }) { b.Width = 105; b.Height = 32; b.Margin = new Padding(5); buttonPanel.Controls.Add(b); }
        btnAdd.Click += btnAdd_Click;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Click += btnDelete_Click;
        btnClear.Click += (_, _) => ClearForm();

        filterLayout.Dock = DockStyle.Fill;
        filterLayout.ColumnCount = 6;
        filterLayout.RowCount = 1;
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        SetupLabel(lblSearch, "Tìm kiếm");
        SetupLabel(lblFilterDate, "Ngày hẹn");
        SetupLabel(lblFilterStatus, "Trạng thái");
        txtSearch.Dock = DockStyle.Fill;
        txtSearch.PlaceholderText = "Nhập tên bệnh nhân hoặc số điện thoại...";
        cboFilterStatus.Dock = DockStyle.Fill;
        cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterStatus.Items.AddRange(new object[] { "Tất cả", "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy" });
        cboFilterStatus.SelectedIndex = 0;
        dtpFilterDate.Dock = DockStyle.Fill;
        filterLayout.Controls.Add(lblSearch, 0, 0); filterLayout.Controls.Add(txtSearch, 1, 0);
        filterLayout.Controls.Add(lblFilterDate, 2, 0); filterLayout.Controls.Add(dtpFilterDate, 3, 0);
        filterLayout.Controls.Add(lblFilterStatus, 4, 0); filterLayout.Controls.Add(cboFilterStatus, 5, 0);
        txtSearch.TextChanged += txtSearch_TextChanged;
        dtpFilterDate.ValueChanged += filterChanged;
        dtpFilterDate.MouseUp += (_, _) => ApplyFilters();
        cboFilterStatus.SelectedIndexChanged += filterChanged;

        dgvAppointments.Dock = DockStyle.Fill;
        dgvAppointments.BackgroundColor = SystemColors.Window;
        dgvAppointments.BorderStyle = BorderStyle.Fixed3D;
        dgvAppointments.RowHeadersVisible = false;
        dgvAppointments.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        dgvAppointments.SelectionChanged += dgvAppointments_SelectionChanged;

        statusStrip.Items.Add(toolStatusLabel);
        toolStatusLabel.Text = "Số lịch hẹn đang hiển thị: 0";
        statusStrip.Dock = DockStyle.Bottom;

        mainLayout.Controls.Add(lblTitle, 0, 0);
        mainLayout.Controls.Add(inputLayout, 0, 1);
        mainLayout.Controls.Add(buttonPanel, 0, 2);
        mainLayout.Controls.Add(filterLayout, 0, 3);
        mainLayout.RowStyles[3].SizeType = SizeType.Absolute;
        mainLayout.RowStyles[3].Height = 48;

        var bodyLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(8) };
        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 355));
        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        bodyLayout.Controls.Add(mainLayout, 0, 0);
        bodyLayout.Controls.Add(dgvAppointments, 0, 1);

        Controls.Add(bodyLayout);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        Text = "Clinic Appointment Manager";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(950, 650);
        Size = new Size(1200, 800);
        Font = new Font("Segoe UI", 9F);
        ((System.ComponentModel.ISupportInitialize)dgvAppointments).EndInit();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private static void SetupLabel(Label label, string text)
    {
        label.Text = text;
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.AutoSize = false;
        label.Margin = new Padding(4);
    }
}
