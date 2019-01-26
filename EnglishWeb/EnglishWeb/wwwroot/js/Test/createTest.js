function init(testType) {
    $(document).ready(() => {
        bind();
    });

    var questionsNumber = 1;

    function bind() {
        $("#createButton").on("click", create);
        $("#newQuestion").on("click", addNewQuestion);

        binCheckBoxed($("#qTemplate1"));
    }

    function binCheckBoxed(template) {
        var checkBoxes = template.find("[type='checkbox']");

        checkBoxes.on("click", e => {
            checkBoxes.prop("checked", false);
            $(e.target).prop("checked", true);
        });
    }

    function create() {
        var model = generateModel();
        var succesMessage = $("#successMessage");
        var errorMessage = $("#errorMessage");

        console.log(model);

        $.ajax({
            method: "POST",
            url: "/Test/Create",
            data: model,
            processData: false,
            contentType: false,
            success: resp => {
                if (resp === "Success") {
	                succesMessage.html("Тест создан успешно");
                    errorMessage.hide();
                    succesMessage.show();
                }
            },
            error: er => {
                errorMessage.show();
                succesMessage.hide();
            }
        });
    }

    function addNewQuestion() {
        var template = $("#qTemplate" + questionsNumber);
        var templateClone = template.clone();

        ++questionsNumber;

        templateClone.find("[q-number]").html("Вопрос " + questionsNumber);
        templateClone.find("[q-number]").attr("q-number", questionsNumber);
        templateClone.attr("id", "qTemplate" + questionsNumber);
        $(".questions").append(templateClone);

        binCheckBoxed(templateClone);
    }

    function generateModel() {
        var model = {
            name: $("#testName").val(),
            questions: [],
            type: testType,
            images:[]
        };
        var form = new FormData();
        form.append("name", model.name);
        form.append("type", model.type);

        for (var i = 1; i < questionsNumber + 1; i++) {
            var template = $("#qTemplate" + i);
            var answersBoxes = template.find(".q-a-box");
            var answers = [];

            for (var j = 0; j < answersBoxes.length; j++) {
                answers.push({
                    text: $(answersBoxes[j]).find(".q-answer").val(),
                    isTrue: $(answersBoxes[j]).find("[type='checkbox']").is(":checked")
                });

                form.append("images", $(answersBoxes[j]).find("[type='file']")[0].files[0]);
            }

            model.questions.push({
                name: template.find(".q-name").val(),
                answers
            });

            form.append("qStringified", JSON.stringify(model.questions));
        }
        return form;
    }
}