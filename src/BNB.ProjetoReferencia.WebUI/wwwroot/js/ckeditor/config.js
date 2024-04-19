/**
 * @license Copyright (c) 2003-2020, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here.
	// For complete reference see:
	// https://ckeditor.com/docs/ckeditor4/latest/api/CKEDITOR_config.html

    config.defaultLanguage = 'pt-br';
    config.language = 'pt-br';

	config.extraPlugins = 'wordcount,table,tableresize,tabletools,tableselection,liststyle,font,indent,indentlist,indentblock,pastetools,imagepaste,pastefromgdocs,pastefromword,simage,filebrowser,popup,filetools,image2';
	config.toolbar = [['Bold', 'Italic', 'Underline']];
	config.removeDialogTabs = 'image:advanced;image:Link;link:advanced;link:upload';

    //Indicates that some of the editor features, like alignment and text direction, should use the "computed value" of the feature to indicate its on/off state instead of using the "real value".
    //If enabled in a Left-To-Right written document, the "Left Justify" alignment button will be shown as active, even if the alignment style is not explicitly applied to the current paragraph in the editor.
    //config.useComputedState = false;
  
	// Remove some buttons provided by the standard plugins, which are
	// not needed in the Standard(s) toolbar.
	config.removeButtons = 'Strikethrough,Subscript,Superscript';
	config.removePlugins = 'chart,elementspath,image';

	// Set the most common block elements.
	config.format_tags = 'p;h1;h2;h3;pre';
	//CKEDITOR.addCss('body{text-align:justify;}');

	// Simplify the dialog windows.
	config.removeDialogTabs = 'image:advanced;link:advanced';
	config.pasteFromWordRemoveFontStyles = false;
	//config.imageUploadURL= 'http://localhost:63410/';
	//config.dataParser = func(data) {
	//    url: 'http://localhost:63410/'
    //};

	config.wordcount = {
	    showWordCount: false,
	    showCharCount: true,
	    maxCharCount: this.element.data('max-length') ? this.element.data('max-length') : 100000
	};

    // Prevent CKEditor disabling a browser's native spell checking feature
	config.disableNativeSpellChecker = false;

	config.height = this.element.data('height') ? this.element.data('height') : "";
	
	//config.removePlugins = 'scayt,wsc';
    //config.removePlugins = 'scayt, wsc';

	//config.scayt_customerId = 'psZ33jI0e6wvrV3';
	//config.scayt_autoStartup = true;
	//config.grayt_autoStartup = true;
	//config.scayt_sLang = 'pt_BR';

	//config.scayt_customerId = '4L3TbAEhdGgIT6p';
	//config.scayt_sLang = 'en_US';
	//config.scayt_disableOptionsStorage = 'lang';

	//config.scayt_autoStartup = true;
	//config.grayt_autoStartup = true;
	//config.scayt_sLang = "en_US";
	//config.scayt_customerId = '4L3TbAEhdGgIT6p';

    //Verificação ortografica
    /*
	config.toolbar_MA = [
    ['Scayt', '-', 'Cut', 'Copy', 'Paste', '-', 'Undo', 'Redo', 'Source'],
	];
	config.disableNativeSpellChecker = false;
	config.defaultLanguage = 'pt';
	config.language = 'pt';

    // Turn on SCAYT automatically
	config.scayt_autoStartup = true;
	config.scayt_sLang = 'pt_BR';
    
	config.scayt_autoStartup = true;
	config.scayt_sLang = 'pt_PT';
	config.scayt_disableOptionsStorage = 'lang';
	config.extraPlugins = 'scayt';
	CKEDITOR.config.scayt_autoStartup = true;
	CKEDITOR.config.grayt_autoStartup = true;
	CKEDITOR.config.scayt_sLang = "pt_PT";*/

};

CKEDITOR.on('instanceReady', function (ev) {
    if (ev.editor.getData() === "") {
        ev.editor.editable().getChild(0).setStyle('text-align', 'justify');
    }
});
