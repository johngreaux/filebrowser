function file(path, name, date, type, size, canCopy, canMove, canDelete) {
    return {
        path: ko.observable(path),
        name: ko.observable(name),
        date: ko.observable(date),
        type: ko.observable(type),
        size: ko.observable(size)
    };
}