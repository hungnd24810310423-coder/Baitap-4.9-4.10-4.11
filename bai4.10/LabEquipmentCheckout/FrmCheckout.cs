using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace LabEquipmentCheckout;

public partial class FrmCheckout : Form
{
    private readonly BindingList<BorrowRecord> _records = new();
    private readonly BindingSource _source = new();
    private bool _loading;
    private string? _selectedId;

    public FrmCheckout()
    {
        InitializeComponent();
        cboEquipment.DataSource = new List<EquipmentItem>
        {
            new() { Ma = "TB01", Ten = "Máy tính" },
            new() { Ma = "TB02", Ten = "Máy chiếu" },
            new() { Ma = "TB03", Ten = "Bàn phím" },
            new() { Ma = "TB04", Ten = "Chuột máy tính" },
            new() { Ma = "TB05", Ten = "Bộ Arduino" },
            new() { Ma = "TB06", Ten = "Router" }
        };
        cboEquipment.DisplayMember = nameof(EquipmentItem.Ten);
        _source.DataSource = _records;
        dgvRecords.AutoGenerateColumns = false;
        dgvRecords.DataSource = _source;
        cboFilter.SelectedIndex = 0;
        dtpBorrow.Value = DateTime.Today;
        dtpDue.Value = DateTime.Today.AddDays(7);
        dtpReturn.Value = DateTime.Today;
        dtpReturn.Checked = false;
        RefreshGrid();
        UpdateStatus();
    }

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        if (_records.Any(r => r.MaPhieu.Equals(txtReceiptId.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            errorProvider.SetError(txtReceiptId, "Mã phiếu đã tồn tại.");
            return;
        }

        _records.Add(ReadInput());
        RefreshGrid();
        ClearInput();
    }

    private void btnUpdate_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        if (string.IsNullOrEmpty(_selectedId))
        {
            MessageBox.Show("Hãy chọn phiếu cần cập nhật trên bảng.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (_records.Any(r => r.MaPhieu.Equals(txtReceiptId.Text.Trim(), StringComparison.OrdinalIgnoreCase)
                              && !r.MaPhieu.Equals(_selectedId, StringComparison.OrdinalIgnoreCase)))
        {
            errorProvider.SetError(txtReceiptId, "Mã phiếu đã tồn tại.");
            return;
        }

        var old = _records.FirstOrDefault(r => r.MaPhieu == _selectedId);
        if (old == null) return;
        int index = _records.IndexOf(old);
        var updated = ReadInput();
        _records[index] = updated;
        _selectedId = updated.MaPhieu;
        RefreshGrid();
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (dgvRecords.CurrentRow?.DataBoundItem is not BorrowRecord record)
        {
            MessageBox.Show("Hãy chọn phiếu cần xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"Bạn có chắc muốn xóa phiếu {record.MaPhieu} không?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        _records.Remove(record);
        _selectedId = null;
        RefreshGrid();
        ClearInput();
    }

    private void btnMarkReturned_Click(object? sender, EventArgs e)
    {
        if (dgvRecords.CurrentRow?.DataBoundItem is not BorrowRecord record)
        {
            MessageBox.Show("Hãy chọn phiếu cần đánh dấu đã trả.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (record.NgayTraThucTe.HasValue)
        {
            MessageBox.Show("Phiếu này đã được trả trước đó.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (DateTime.Today < record.NgayMuon.Date)
        {
            MessageBox.Show("Ngày trả thực tế không được trước ngày mượn.", "Lỗi dữ liệu",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        record.NgayTraThucTe = DateTime.Today;
        _source.ResetBindings(false);
        LoadRecord(record);
        RefreshGrid();
    }

    private void btnSearch_Click(object? sender, EventArgs e) => RefreshGrid();

    private void btnClearSearch_Click(object? sender, EventArgs e)
    {
        txtSearch.Clear();
        cboFilter.SelectedIndex = 0;
        RefreshGrid();
    }

    private void cboFilter_SelectedIndexChanged(object? sender, EventArgs e) => RefreshGrid();

    private void btnExport_Click(object? sender, EventArgs e)
    {
        RefreshGrid();
        using SaveFileDialog dialog = new()
        {
            Title = "Xuất danh sách phiếu mượn",
            Filter = "CSV (*.csv)|*.csv",
            FileName = "PhieuMuonThietBi.csv",
            AddExtension = true,
            DefaultExt = "csv"
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;

        var rows = GetFilteredRecords().ToList();
        var sb = new StringBuilder();
        sb.AppendLine("MaPhieu,MaSinhVien,HoTen,ThietBi,SoLuong,NgayMuon,HanTra,NgayTraThucTe,TinhTrang");
        foreach (var r in rows)
        {
            sb.AppendLine(string.Join(",", new[]
            {
                Csv(r.MaPhieu), Csv(r.MaSinhVien), Csv(r.HoTen), Csv(r.ThietBi),
                r.SoLuong.ToString(CultureInfo.InvariantCulture),
                r.NgayMuon.ToString("yyyy-MM-dd"), r.HanTra.ToString("yyyy-MM-dd"),
                r.NgayTraThucTe?.ToString("yyyy-MM-dd") ?? "", Csv(r.TinhTrang)
            }));
        }
        File.WriteAllText(dialog.FileName, "\uFEFF" + sb.ToString(), new UTF8Encoding(true));
        MessageBox.Show($"Đã xuất {rows.Count} phiếu ra CSV.", "Thành công",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static string Csv(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";

    private BorrowRecord ReadInput()
    {
        return new BorrowRecord
        {
            MaPhieu = txtReceiptId.Text.Trim(),
            MaSinhVien = txtStudentId.Text.Trim(),
            HoTen = txtStudentName.Text.Trim(),
            ThietBi = cboEquipment.Text.Trim(),
            SoLuong = (int)nudQuantity.Value,
            NgayMuon = dtpBorrow.Value.Date,
            HanTra = dtpDue.Value.Date,
            NgayTraThucTe = dtpReturn.Checked ? dtpReturn.Value.Date : null
        };
    }

    private bool ValidateInput()
    {
        errorProvider.Clear();
        bool ok = true;
        if (string.IsNullOrWhiteSpace(txtReceiptId.Text))
        {
            errorProvider.SetError(txtReceiptId, "Vui lòng nhập mã phiếu.");
            ok = false;
        }
        if (string.IsNullOrWhiteSpace(txtStudentId.Text))
        {
            errorProvider.SetError(txtStudentId, "Vui lòng nhập mã sinh viên.");
            ok = false;
        }
        if (string.IsNullOrWhiteSpace(txtStudentName.Text))
        {
            errorProvider.SetError(txtStudentName, "Vui lòng nhập họ tên sinh viên.");
            ok = false;
        }
        if (cboEquipment.SelectedIndex < 0 || string.IsNullOrWhiteSpace(cboEquipment.Text))
        {
            errorProvider.SetError(cboEquipment, "Vui lòng chọn thiết bị.");
            ok = false;
        }
        if (nudQuantity.Value <= 0)
        {
            errorProvider.SetError(nudQuantity, "Số lượng phải lớn hơn 0.");
            ok = false;
        }
        if (dtpDue.Value.Date < dtpBorrow.Value.Date)
        {
            errorProvider.SetError(dtpDue, "Hạn trả không được trước ngày mượn.");
            ok = false;
        }
        if (dtpReturn.Checked && dtpReturn.Value.Date < dtpBorrow.Value.Date)
        {
            errorProvider.SetError(dtpReturn, "Ngày trả thực tế không được trước ngày mượn.");
            ok = false;
        }
        return ok;
    }

    private IEnumerable<BorrowRecord> GetFilteredRecords()
    {
        IEnumerable<BorrowRecord> query = _records;
        string keyword = txtSearch.Text.Trim();
        if (keyword.Length > 0)
        {
            query = query.Where(r => r.MaSinhVien.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || r.ThietBi.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
        string filter = cboFilter.SelectedItem?.ToString() ?? "Tất cả";
        if (filter != "Tất cả") query = query.Where(r => r.TinhTrang == filter);
        return query;
    }

    private void RefreshGrid()
    {
        var currentId = dgvRecords.CurrentRow?.DataBoundItem is BorrowRecord current
            ? current.MaPhieu : _selectedId;
        _source.DataSource = new BindingList<BorrowRecord>(GetFilteredRecords().ToList());
        dgvRecords.DataSource = _source;
        if (currentId != null)
        {
            foreach (DataGridViewRow row in dgvRecords.Rows)
            {
                if (row.DataBoundItem is BorrowRecord r && r.MaPhieu == currentId)
                {
                    row.Selected = true;
                    dgvRecords.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }
        UpdateStatus();
    }

    private void dgvRecords_SelectionChanged(object? sender, EventArgs e)
    {
        if (_loading || dgvRecords.CurrentRow?.DataBoundItem is not BorrowRecord record) return;
        LoadRecord(record);
    }

    private void LoadRecord(BorrowRecord r)
    {
        _loading = true;
        _selectedId = r.MaPhieu;
        txtReceiptId.Text = r.MaPhieu;
        txtStudentId.Text = r.MaSinhVien;
        txtStudentName.Text = r.HoTen;
        cboEquipment.Text = r.ThietBi;
        nudQuantity.Value = Math.Max(nudQuantity.Minimum, Math.Min(nudQuantity.Maximum, r.SoLuong));
        dtpBorrow.Value = r.NgayMuon;
        dtpDue.Value = r.HanTra;
        dtpReturn.Checked = r.NgayTraThucTe.HasValue;
        dtpReturn.Value = r.NgayTraThucTe ?? DateTime.Today;
        _loading = false;
    }

    private void ClearInput()
    {
        _loading = true;
        _selectedId = null;
        txtReceiptId.Clear();
        txtStudentId.Clear();
        txtStudentName.Clear();
        cboEquipment.SelectedIndex = -1;
        nudQuantity.Value = 1;
        dtpBorrow.Value = DateTime.Today;
        dtpDue.Value = DateTime.Today.AddDays(7);
        dtpReturn.Value = DateTime.Today;
        dtpReturn.Checked = false;
        errorProvider.Clear();
        _loading = false;
        txtReceiptId.Focus();
    }

    private void UpdateStatus()
    {
        int overdue = _records.Count(r => r.TinhTrang == "Quá hạn");
        toolStripStatusLabel.Text = $"Tổng số phiếu: {_records.Count}   |   Số phiếu quá hạn: {overdue}";
    }
}
