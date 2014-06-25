angular.module('blog').filter('decode', function($window){
	return function(input){

		return input ? $window.atob(input) : '';
	}
})