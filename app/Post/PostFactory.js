angular.module('blog').factory('PostFactory', function($http){
	var PostFactory = {};

	PostFactory.post = {};

	PostFactory.get = function(date, name){
		var url = "https://api.github.com/repos/yngvebn/blog.yngenilsen.com-content/contents/Posts/"+date+"/"+name;
		return $http.get(url).then(function(data){
			PostFactory.post = data.data;
			console.log(PostFactory.post);
		});
	}

	return PostFactory;
})