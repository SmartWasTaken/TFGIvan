mergeInto(LibraryManager.library, {
  	JS_CerrarJuego: function () {
    		CerrarJuegoDesdeUnity();
  	},

	JS_GetToken: function(){
		var returnStr = window.userToken || "NO_TOKEN";
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
});