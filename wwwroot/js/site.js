// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Client-side validator for MustBeTrueAttribute
(function ($) {
	if (!$ || !$.validator || !$.validator.unobtrusive) return;
	$.validator.addMethod('mustbetrue', function (value, element) {
		if (element.type === 'checkbox') return element.checked === true;
		return value === 'true' || value === true;
	});
	$.validator.unobtrusive.adapters.addBool('mustbetrue');
})(window.jQuery);
