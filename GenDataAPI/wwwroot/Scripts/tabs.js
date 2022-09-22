function Tabs() {

}

Tabs.prototype = {
    showTrace: function (message) {
    // console.log('showoutput');
        $('.cntoutput').hide();
        $('.cntdebug').show();
        $('.cnterror').hide();
    },
    showError: function (message) {
    //    console.log('showError');
        $('.cntoutput').hide();
        $('.cntdebug').hide();
        $('.cnterror').show();
    },

    showOutput: function (message) {    
    //    console.log('showTrace');
        $('.cntoutput').show();
        $('.cntdebug').hide();
        $('.cnterror').hide();
    },
    run: function () {
        console.log('run');
        $('.cntoutput').show();
        $('.cntdebug').hide();
        $('.cnterror').hide();
    }


}