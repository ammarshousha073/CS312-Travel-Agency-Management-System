# CS312 implementation evidence

| Requirement | Actual implementation | Source file | Status |
| --- | --- | --- | --- |
| Windows Forms | Login and Dashboard, navigation across six management modules, shared grid and filters, reusable input dialogs. | [Login Designer](TravelAgencyManagementSystem/Forms/LoginForm.Designer.cs), [main navigation](TravelAgencyManagementSystem/Forms/MainForm.cs), [records Designer](TravelAgencyManagementSystem/UserControls/RecordsPanel.Designer.cs) | Complete by source inspection |
| Event handling | Sign-in and navigation clicks, filters, grid double-click, record actions and dialog saves. Selection changes recalculate booking totals and payment balances. | [Records panel](TravelAgencyManagementSystem/UserControls/RecordsPanel.cs), [record dialog](TravelAgencyManagementSystem/Forms/Dialogs/RecordDialog.cs), [bookings](TravelAgencyManagementSystem/UserControls/BookingsControl.cs), [payments](TravelAgencyManagementSystem/UserControls/PaymentsControl.cs) | Complete by source inspection |
| GDI+ graphics | Paint handlers and OnPaint overrides use Graphics, Pen, and Brush with DrawLine, DrawRectangle, FillRectangle, and FillEllipse. The branding illustration uses TranslateTransform and RotateTransform. | [Dashboard](TravelAgencyManagementSystem/UserControls/DashboardControl.cs), [brand panel](TravelAgencyManagementSystem/UserControls/TravelBrandPanel.cs), and the charts below | Complete by source inspection |
| Visualization | Dashboard booking line chart and monthly revenue bar chart with custom markers, guides, and labels. | [Bookings chart](TravelAgencyManagementSystem/UserControls/BookingsChart.cs), [revenue chart](TravelAgencyManagementSystem/UserControls/RevenueChart.cs), [dashboard Designer](TravelAgencyManagementSystem/UserControls/DashboardControl.Designer.cs) | Complete by source inspection |

The GDI+ code uses FillEllipse for markers. Source inspection did not find direct calls to DrawEllipse or DrawImage.
