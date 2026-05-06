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

