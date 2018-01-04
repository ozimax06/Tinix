module Tinix {

	declare var $: any;

	declare var tinymce: any;

	$(document).ready(DoInitialSetup);


	function DoInitialSetup() {

		SetupEditor();

		$("#linkLogOff").click(OnLogOff);

		$("#buttonPost").click(OnPostNew);

		$(".deleteButton").click(DeletePost);

	}


	function DeletePost(e) {

		if (confirm("Are you absolutely sure you want to delete this ? ")) {

			var id: string = $(e.target).attr("data-id");

			$("#fieldDelete").val(id);

			$("#formDelete").submit();
		}
	}


	function OnPostNew() {

		var content = tinymce.get('Content').getContent({ format: 'text' });

		if ($.trim(content) == '') {
			alert("Please add the blog post");
			return false;
		}

		return true;
	}


	function OnLogOff() {

		if (confirm("Log off from administration area ? ")) {
			$("#formLogOff").submit();
		}
	}

	function SetupEditor() {

		// Setup editor
		var editPost = document.getElementById("Content");

		tinymce.init({
			selector: '#Content',
			autoresize_min_height: 200,
			plugins:
			'autosave preview searchreplace visualchars image link media fullscreen code table hr pagebreak autoresize nonbreaking anchor insertdatetime advlist lists textcolor wordcount imagetools colorpicker',
			menubar: "edit view format insert table",
			toolbar1:
			'formatselect | bold italic blockquote forecolor backcolor | link | alignleft aligncenter alignright  | numlist bullist outdent indent | fullscreen',
			selection_toolbar: 'bold italic | quicklink h2 h3 blockquote',
			autoresize_bottom_margin: 0,
			paste_data_images: true,
			image_advtab: true,
			file_picker_types: 'image',
			relative_urls: false,
			convert_urls: false,
			branding: false,

			//setup: function (editor) {
			//	editor.addButton('imageupload',
			//		{
			//			icon: "image",
			//			onclick: function () {
			//				var fileInput = document.createElement("input");
			//				fileInput.type = "file";
			//				fileInput.multiple = true;
			//				fileInput.accept = "image/*";
			//				fileInput.addEventListener("change", handleFileSelect, false);
			//				fileInput.click();
			//			}
			//		});

			//}
		});


		//// Delete post
		//var deleteButton = edit.querySelector(".delete");

		//deleteButton.addEventListener("click",
		//	function(e) {
		//		if (!confirm("Are you sure you want to delete the post?")) {
		//			e.preventDefault();
		//		}
		//	});

	}

}
