function init(testType, language, id, questionsNumber = 1) {
    $(document).ready(() => {
        bind();
    });

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
        const model = generateModel();
        var successMessage = $("#successMessage");
        var errorMessage = $("#errorMessage");
        const checkingResult = checkAllFields();

        if (!checkingResult.status) {
            successMessage.hide();
            errorMessage.html(checkingResult.message);
            errorMessage.show();

            return;
        } else {
            errorMessage.hide();
        }

        console.log(model);

        $.ajax({
            method: "PUT",
            url: "/Test/Edit",
            data: model,
            processData: false,
            contentType: false,
            success: resp => {
                if (resp === "Success") {
                    successMessage.html("Тест обновлен успешно");
                    errorMessage.hide();
                    successMessage.show();
                }
            },
            error: er => {
                errorMessage.html(er.responseText);
                errorMessage.show();
                successMessage.hide();
            }
        });
    }

    function addNewQuestion() {
        const template = $("#qTemplate" + questionsNumber);
        const templateClone = template.clone();

        ++questionsNumber;

        templateClone.find("[q-number]").html("Вопрос " + questionsNumber);
        templateClone.find("[q-number]").attr("q-number", questionsNumber);
        templateClone.attr("id", "qTemplate" + questionsNumber);
        templateClone.find(".q-name").val("");
        templateClone.find(".q-name").attr("q-id", "00000000-0000-0000-0000-000000000000");
        templateClone.find(".q-answer").val("");
        templateClone.find(".q-answer").attr("q-ansver-id", "00000000-0000-0000-0000-000000000000");
        $(".questions").append(templateClone);

        binCheckBoxed(templateClone);
    }

    function generateModel() {
        const model = {
            id: id,
            name: $("#testName").val(),
            questions: [],
            type: testType,
            images: []
        };
        const form = new FormData();

        form.append("name", model.name);
        form.append("type", model.type);
        form.append("language", language);
        form.append("id", model.id);

        for (let i = 1; i < questionsNumber + 1; i++) {
            const template = $(`#qTemplate${i}`);
            const answersBoxes = template.find(".q-a-box");
            const answers = [];

            for (let j = 0; j < answersBoxes.length; j++) {
                answers.push({
                    id: $(answersBoxes[j]).find(".q-answer").attr("q-ansver-id"),
                    text: $(answersBoxes[j]).find(".q-answer").val(),
                    isTrue: $(answersBoxes[j]).find("[type='checkbox']").is(":checked")
                });

                testType === 1 && form.append("images", $(answersBoxes[j]).find("[type='file']")[0].files[0]);
            }

            model.questions.push({
                id: template.find(".q-name").attr("q-id"),
                name: template.find(".q-name").val(),
                answers
            });
        }
        form.append("qStringified", JSON.stringify(model.questions));

        return form;
    }

    function checkAllFields() {
        if (!checkField($("#testName").val()))
            return {
                message: "Please enter test name. Test name length must be > 2",
                status: false
            };

        const templates = $(".q-template");

        if (!templates.length)
            return {
                message: "Questions was not found. Pease add question",
                status: false
            };

        for (let i = 0; i < templates.length; i++) {
            const questionName = $(templates[i]).find(".q-name").val();

            if (!checkField(questionName))
                return {
                    message: "Please fill all question names. Question name length must be > 2",
                    status: false
                };

            const answers = $(templates[i]).find(".q-answer");

            // if not image test
            if (testType !== 1) {
                for (let j = 0; j < answers.length; j++) {
                    if (!checkField($(answers[j]).val()))
                        return {
                            message: "Please fill all answers. Answer name length must be > 2",
                            status: false
                        };
                }
            }

            var checkBoxes = $(templates[i]).find("[type='checkbox']");

            if (!checkBoxes.toArray().some(cb => $(cb).is(":checked")))
                return {
                    message: "Please check all checkboxes",
                    status: false
                };
        }

        // check if images
        if (testType === 1 && $("[type='file']").toArray().some(file => !file.files.length)) {
            return {
                message: "Please choose all images",
                status: false
            };
        }


        return {
            message: "Success",
            status: true
        };
    }

    const checkField = val => val && val.length > 2;
}