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
            destroy: {
                url: "/Home/DestroyPurchase",
                dataType: "json",
                type: "POST"
            },
            create: {
                url: "/Home/CreatePurchase",
                dataType: "json",
                type: "POST"
            },
            parameterMap: function(options, operation) {
                if (operation !== "read" && options.models) {
                    return {models: kendo.stringify(options.models)};
                }
            }
        },
        error: function (e) {
            if (e.status == "customerror") {
                alert("Error: " + e.errors);
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
                    amount: { type: "number", validation: { required: true } },
                    payee: { type: "string", validation: { required: true } },
                    memo: { type: "string" }
                }
            }
        },
        sort: { field: "date", dir: "desc" },
        change: function(e) {
            var aggString;
            var dataObj = this.data();
            var count = dataObj.length;
            if (count) {
                var sum = 0;
                for (var i = 0; i < dataObj.length; i++) {
                    sum += dataObj[i].amount;
                }
                var avg = 1.0 * sum / count;
                aggString = "Purchases: " + count + " | Purchase sum: " + sum.toFixed(2) + " | Purchase average: " + avg;
            }
            else
            {
                aggString = "No purchases to view";
            }
            $("#aggregates").text(aggString);
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
        $("#edit-category").kendoDropDownList({
            dataSource: categories,
            dataTextField: "name",
            dataValueField: "id"
         }).data("kendoDropDownList");

        $("#edit-date").kendoDatePicker();
    });

    $("#add-purchase").click(function(e) {
        var purchaseList = $("#purchase-list").data("kendoListView");
        purchaseList.add();
    });
});