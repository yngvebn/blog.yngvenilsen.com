angular.module('blog').factory('PostFactory', function($q, $http, common, decodeFilter){
	var PostFactory = {};
	PostFactory.post = {
		title: '', 
		tags: [],
		body: ''
	}

	PostFactory.get = function(date, name){
		var urls = [$http.get("https://api.github.com/repos/yngvebn/blog.yngenilsen.com-content/contents/Posts/"+date+"/"+name+".md"), 
		$http.get("https://api.github.com/repos/yngvebn/blog.yngenilsen.com-content/contents/Posts/"+date+"/"+name+".json")];
		return $q.all(urls).then(function(response){
			post = response[0].data;
			settings = JSON.parse(decodeFilter(response[1].data.content));
			
			PostFactory.post.title = settings.title;
			PostFactory.post.tags = settings.tags;
			PostFactory.post.body = post.content;

		});
	}

	return PostFactory;
})