angular.module('blog').factory('PostsFactory', function($q, $http, common){
	var PostsFactory = {}
	
	var rootUrl = 'https://api.github.com/repos/yngvebn/blog.yngenilsen.com-content/contents';

	PostsFactory.posts = [];
	
	PostsFactory.getPosts = function(){
	    return $http.get(rootUrl+'/Posts').then(function(posts){
			var urls = _.chain(posts.data).flatten('path').map(function(path){ return $http.get(rootUrl+'/'+path)}).value();
			return $q.all(urls).then(function(){
				var arr = _.chain(arguments).flatten().value();

				PostsFactory.posts = _.chain(arr).flatten('data').where(function(data) {  return common.endsWith(data.name, '.json') }).map(function(item) { 
					item.path = common.stripExtension(item.path);
					item.name = common.stripExtension(item.name);
				 	return item;
				 }).value();

				console.log(PostsFactory.posts);
			})	
		});
	};

	return PostsFactory;
})