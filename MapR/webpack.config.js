const path = require('path');

module.exports = {
    mode: "development",
    entry: {
        //could probably function based thing to pull in a ll js files
        gamePlayer: './Features/PlayGame/js/GamePlayer.js',
        gameAdmin: './Features/PlayGame/js/GameAdmin.js'
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot/js'),
        library: '[name]',
        libraryExport: 'default',
        libraryTarget: 'var'
    }
};