window.ReportingLocalization = {
    onCustomizeLocalization: function(s, e) {
        $.get("/js/localization/vi.json").done(result => {
            e.WidgetLocalization.loadMessages(result);
        }).always(() => {
            e.WidgetLocalization.locale("vi");
        })

        e.LoadMessages($.get("/js/localization/dx-analytics-core.vi.json"));
        e.LoadMessages($.get("/js/localization/dx-reporting.vi.json"));
    }
}