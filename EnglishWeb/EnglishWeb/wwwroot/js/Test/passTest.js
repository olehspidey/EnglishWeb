function init(testType) {
    $(document).ready(() => {
        bind();
    });

    function bind() {
        $("#submitBut").on("click", submit);

        var radioGroups = $("[radiogroup]");

        radioGroups.find("[type='radio']").on("click", e => {
            $(e.target).closest("[radiogroup]").find("[type='radio']").prop("checked", false);
            $(e.target).prop("checked", true);
        });
    }

    function submit() {
        var succesMessage = $("#successMessage");
        var errorMessage = $("#errorMessage");
        var checkedResult = false;
        var model = null;

        if (testType === 0 || testType === 1) {
            checkedResult = checkRadioAndImageModel();
            model = generateModel();
        }

        if (testType === 2) {
            checkedResult = checkInputModel();
            model = generateInputModel();
        }

        if (checkedResult) {
            errorMessage.hide();
        }
        else {
            succesMessage.hide();
            errorMessage.show();
            errorMessage.html("Нужно ответить на все вопросы");
            return;
        }
        $.ajax({
            method: "POST",
            url: "/Test/Pass",
            data: model,
            success: e => {
                errorMessage.hide();
                succesMessage.html("Тест успешно сдан");
                succesMessage.show();

                $("#trueCount").html(`Правильных ответов ${e.trueCount}`);
                $("#falseCount").html(`Неправильных ответов ${e.falseCount}`);
                $("#qestionsCount").html(`Общее количество вопросов ${e.questionsCount}`);
                $("#result").show();
            },
            error: er => {
                succesMessage.hide();
                errorMessage.show();
                errorMessage.html("Error");
            }
        });
    }

    function generateModel() {
        var radioGroups = $("[radiogroup]");
        var model = {
            id: $("[test-id]").attr("test-id"),
            answersId: [],
            type: testType
        };

        for (var i = 0; i < radioGroups.length; i++) {
            var radioGroup = $(radioGroups[i]);

            model.answersId.push(radioGroup.find(":checked").attr("name"));
        }

        return model;
    }

    function generateInputModel() {
        var inputs = $("input");
        var model = {
            id: $("[test-id]").attr("test-id"),
            answers: [],
            type: testType
        };

        for (var i = 0; i < inputs.length; i++) {
            model.answers.push($(inputs[i]).val());
        }

        return model;
    }

    function checkRadioAndImageModel() {
        var radioGroups = $("[radiogroup]");
        var res = true;

        radioGroups.toArray()
            .forEach(rg => {
                if (!$(rg).find("[type='radio']").toArray().some(radio => $(radio).is(":checked"))) {
                    res = false;

                    return;
                }
            });

        return res;
    }

    function checkInputModel() {
        return !$("input").toArray().some(input => !$(input).val().length);
    }
}