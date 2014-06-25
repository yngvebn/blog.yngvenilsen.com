angular.module('blog').controller('FrontPage', function(PostsFactory, $scope){

	this.greeting = 'Hello, blog';
	this.posts = PostsFactory.posts;

	PostsFactory.getPosts().then(function(){
		this.posts = PostsFactory.posts;
	}.bind(this));
});