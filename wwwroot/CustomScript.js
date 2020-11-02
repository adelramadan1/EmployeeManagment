function ConfirmDelete(UniqueId, IsTrue) {
    var deleteSpan = 'deletespan_' + UniqueId;
    var confirmDeteteSpan = 'confirmdeletespan_' + UniqueId;

    if (IsTrue) {
        $('#'+deleteSpan).hide();
        $('#' + confirmDeteteSpan).show();

    }
    else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeteteSpan).hide();
    }

}