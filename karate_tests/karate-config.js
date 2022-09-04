function() {
    var baseUrl = java.lang.System.getenv('BASE_URL');
    if(!baseUrl){
        baseUrl = 'http://localhost:5175'
    }
    var config = {
      baseUrl: baseUrl
    }
    karate.log('config:', config);
    return config;
}