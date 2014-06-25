angular.module('blog').controller('Post',  function(PostFactory, $routeParams){
	this.current = {};

	PostFactory.get($routeParams.date, $routeParams.name).then(function(){
		this.current = PostFactory.post;
	}.bind(this));
})