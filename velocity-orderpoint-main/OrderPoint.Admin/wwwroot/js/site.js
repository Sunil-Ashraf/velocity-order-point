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
	window.ShowSwal = function (title, message, icon) {
		Swal.fire({
			title: title,
			text: message,
			icon: icon,
			confirmButtonText: 'OK'
		});
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

//window.initializeSelect2 = (elementId) => {
//	$('#' + elementId).select2({
//		width: '100%',
//		placeholder: "Select a user",
//		//allowClear: true
//	});
//};
window.initializeSelect2 = (selectId, dotNetHelper) => {
	const selector = `#${selectId}`;

	if ($(selector).data('select2')) {
		$(selector).select2('destroy');
	}

	$(selector).select2({
		width: '100%',
		placeholder: '-- Select a user --',
		allowClear: true
	});

	// Important: Hook up the 'change' event AFTER initialization
	$(selector).on('change', function () {
		const selectedValue = $(this).val();

		// Call the Blazor method (async)
		if (dotNetHelper) {
			dotNetHelper.invokeMethodAsync('OnUserChangedFromJS', selectedValue);
		}
	});
};

window.applyPhoneMask = (elementId) => {
	$(document).ready(function () {
		 
		$('.' + elementId).mask('(000) 000-0000');
		 
	});
};