$(document).ready(function () {

	window.showModal = (modalId) => {

		let modal = new bootstrap.Modal(document.getElementById(modalId));
		modal.show();

		setTimeout(() => {
			//updateBackdrop();
		}, 100);
	};

	window.hideModal = (modalId, isNested = false) => {

		let modalElement = document.getElementById(modalId);
		if (modalElement) {
			let modal = bootstrap.Modal.getInstance(modalElement);
			if (modal) {
				modal.hide();
			}
		}
	} 
	
	
	
});
function insertTextAtCursor(text) {
	tinymce.activeEditor.execCommand('mceInsertContent', false, text);
}


window.ShowConfirmSwal = async function (title, message, icon) {
	const result = await Swal.fire({
		title: title,
		text: message,
		icon: icon,
		showCancelButton: true,
		confirmButtonText: 'Yes',
		cancelButtonText: 'Cancel'
	});

	// Return only a boolean (true/false)
	return result.isConfirmed === true;
};
window.ShowSwal = function (title, message, icon) {
	Swal.fire({
		title: title,
		text: message,
		icon: icon,
		confirmButtonText: 'OK'
	});
};
document.querySelectorAll('.checkout a').forEach(function (el) {
	new bootstrap.Tooltip(el, {
		placement: 'top'
	});
});

window.registerOutsideClick = function (element, dotnetHelper) {
	function handler(event) {
		if (element && !element.contains(event.target)) {
			dotnetHelper.invokeMethodAsync("CloseMenu");
		}
	}
	document.addEventListener("mousedown", handler);
	window._outsideClickHandler = handler;
};

window.unregisterOutsideClick = function () {
	if (window._outsideClickHandler) {
		document.removeEventListener("mousedown", window._outsideClickHandler);
		window._outsideClickHandler = null;
	}
};

window.initTimePicker = (elementId) => {
	flatpickr("#" + elementId, {
		enableTime: true,
		noCalendar: true,
		dateFormat: "H:i", // 24h format
		time_24hr: true,
		onChange: function (selectedDates, dateStr) {
			DotNet.invokeMethodAsync("YourAssemblyName", "SetOrderTime", dateStr);
		}
	});
};
window.applyPhoneMask = (elementId) => {
	$(document).ready(function () {

		$('.' + elementId).mask('(000) 000-0000');

	});
};