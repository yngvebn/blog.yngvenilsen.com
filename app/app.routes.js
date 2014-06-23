angular.module('blog').config(function($routeProvider, $locationProvider){
	$routeProvider.when('/', {
		templateUrl: '/app/FrontPage/FrontPage.html',
		controller: 'FrontPage',
		controllerAs: 'frontPage'
	})
	.when('/Posts/:date/:name', {
		templateUrl: '/app/Post/Post.html',
		controller: 'Post',
		controllerAs: 'post'
	}).otherwise({ redirectTo: '/'});

	$locationProvider.html5Mode(true);
})