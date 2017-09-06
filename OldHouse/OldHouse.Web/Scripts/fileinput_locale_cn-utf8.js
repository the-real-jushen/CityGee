/*!
 * FileInput <_LANG_> Translations
 *
 * This file must be loaded after 'fileinput.js'. Patterns in braces '{}', or
 * any HTML markup tags in the messages must not be converted or translated.
 *
 * @see http://github.com/kartik-v/bootstrap-fileinput
 *
 * NOTE: this file must be saved in UTF-8 encoding.
 */
(function ($) {
    "use strict";

    $.fn.fileinput.locales._LANG_ = {
        fileSingle: '文件',
        filePlural: '文件',
        browseLabel: '浏览 &hellip;',
        removeLabel: '移除',
        removeTitle: '清除所选文件',
        cancelLabel: '取消',
        cancelTitle: '终止本次上传',
        uploadLabel: '上传',
        uploadTitle: '上传所选文件',
        msgSizeTooLarge: '文件 "{name}" (<b>{size} KB</b>) 太大了！！超过了 <b>{maxSize} KB</b>. 请您重试一下!',
        msgFilesTooLess: '您必须至少选择 <b>{n}</b> {files} . 请您重试一下!',
        msgFilesTooMany: '您选择了太多的文件， <b>({n})</b> 超过了限制 <b>{m}</b>. Please retry your upload!',
        msgFileNotFound: '文件 "{name}" 找不到!',
        msgFileSecured: 'Security restrictions prevent reading the file "{name}".',
        msgFileNotReadable: 'File "{name}" is not readable.',
        msgFilePreviewAborted: 'File preview aborted for "{name}".',
        msgFilePreviewError: 'An error occurred while reading the file "{name}".',
        msgInvalidFileType: 'Invalid type for file "{name}". Only "{types}" files are supported.',
        msgInvalidFileExtension: 'Invalid extension for file "{name}". Only "{extensions}" files are supported.',
        msgValidationError: 'File Upload Error',
        msgLoading: 'Loading file {index} of {files} &hellip;',
        msgProgress: 'Loading file {index} of {files} - {name} - {percent}% completed.',
        msgSelected: '{n} files selected',
        msgFoldersNotAllowed: '不可以放文件夹哦， {n} 个文件夹被忽略.',
        dropZoneTitle: 'Drag & drop files here 往这里拖拽试试 &hellip;'
    };

    $.extend($.fn.fileinput.defaults, $.fn.fileinput.locales._LANG_);
})(window.jQuery);