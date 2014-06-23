angular.module('blog').factory('common', function(){
	 function stripExtension(input){
	 	var lastIndex = input.lastIndexOf('.');
	 	var extensionLength = input.length - lastIndex;
		if(input && lastIndex > -1){
			return input.slice(0, lastIndex);
		}
		return input;
	}

	function endsWith(input, tail){
		return input && input.substring(input.length- tail.length) === tail;
	}

	return {
		stripExtension: stripExtension,
		endsWith: endsWith
	}
})