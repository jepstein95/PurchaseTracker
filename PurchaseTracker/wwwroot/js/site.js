$(document).ready(function () {
    var purchases = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/Home/GetPurchases",
                dataType: "json",
                type: "GET"
            },
            update: {
                url: "/Home/UpdatePurchase",
                dataType: "json",
                type: "POST"
            },
            delete: {
                url: "/Home/DeletePurchase",
                dataType: "json",
                type: "POST"
            },
            create: {url: "/Home/CreatePurchase",
                dataType: "json",
                type: "POST"
            },
            parameterMap: function(options, operation) {
                if (operation !== "read" && options.models) {
                    return {models: kendo.stringify(options.models)};
                }
            }
        },
        batch: true,
        schema: {
            model: {
                id: "id",
                fields: {
                    id: { type: "number", editable: false, nullable: true },
                    categoryId: { type: "number", validation: { required: true } },
                    categoryName: { type: "string" },
                    date: { type: "date", validation: { required: true } },
                    payee: { type: "string", validation: { required: true } },
                    memo: { type: "string"}
                }
            }
        }
    });

    var categories = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/Home/GetCategories",
                dataType: "json",
                type: "GET"
            }
        },
        schema: {
            model: {
                id: "id",
                fields: {
                    id: { type: "number", editable: false },
                    name: { type: "string", editable: false }
                }
            }
        }
    });
    
    categories.read();

    var purchaseList = $("#purchase-list").kendoListView({
        dataSource: purchases,
        template: kendo.template($("#view-template").html()),
        editTemplate: kendo.template($("#edit-template").html())
    }).data("kendoListView");

    purchaseList.bind("edit", function(e) {
        var categoryList = $("#edit-category").kendoDropDownList({
            dataSource: categories,
            dataTextField: "name",
            dataValueField: "id"
         }).data("kendoDropDownList");

        var datePicker = $("#edit-date").kendoDatePicker();
    });

    $(document).on('click','.add-button', function(e) {
        var id = $(e.target).data("id");
        window.e = e;
    });
});