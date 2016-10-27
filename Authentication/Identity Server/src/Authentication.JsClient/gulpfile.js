/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');

gulp.task('copy-vendor', function () {
    gulp.src([
        './node_modules/jquery/dist/jquery.min.js',
        './node_modules/oidc-client/dist/oidc-client.min.js',
        './node_modules/bootstrap/dist/js/bootstrap.min.js'
    ])
        .pipe(gulp.dest('./wwwroot/js'));

    gulp.src(['./node_modules/bootstrap/dist/css/bootstrap.min.css'])
        .pipe(gulp.dest('./wwwroot/css'));
});

gulp.task('copy-html', function () {
    gulp.src(['./html/**/*.html'])
        .pipe(gulp.dest('./wwwroot'));
});

gulp.task('default', ['copy-vendor']);