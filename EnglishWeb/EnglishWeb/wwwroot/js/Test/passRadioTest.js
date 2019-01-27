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

    if (!checkModel()) {
        succesMessage.hide();
        errorMessage.show();
        errorMessage.html("Нужно ответить на все вопросы");
        return;
    }

    var model = generateModel();

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
        answersId: []
    };

    for (var i = 0; i < radioGroups.length; i++) {
        var radioGroup = $(radioGroups[i]);

        model.answersId.push(radioGroup.find(":checked").attr("name"));
    }

    return model;
}

function checkModel() {
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