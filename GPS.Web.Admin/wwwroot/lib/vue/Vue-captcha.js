var Childcomp = `<div>
     <div >
                <div >
                    <canvas id="CapCode"  width="150" height="44" style="border-radius: 4px;"></canvas>
                    
                    <button type="button" class="btn btn-default btn-sm" @click="UpdateCaptcha()"><i class="fas fa-sync-alt"></i></button>
                </div>
               <div style="display:none">
                <span>
                    <input type="text"  v-model="EnteredValue" />
                    <input type="button"  @click="ValidateCaptcha()" value="Submit">
                </span>
                <br>
                <div v-if="ValidationSection">
                    <span  >Error</span>
                    <span ">success</span>
                </div>
            </div>
</div>
</div>`;
Vue.component('vue-captcha', {

    data() {
        return {
            cd: [],
            EnteredValue: null,
            ValidationSection: false,
            valid: false
        }
    },
    methods: {
        CreateCaptcha: function () {
            var alpha = new Array(
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9');

            var a = alpha[Math.floor(Math.random() * alpha.length)];
            var b = alpha[Math.floor(Math.random() * alpha.length)];
            var c = alpha[Math.floor(Math.random() * alpha.length)];
            var d = alpha[Math.floor(Math.random() * alpha.length)];
            var e = alpha[Math.floor(Math.random() * alpha.length)];
            var f = alpha[Math.floor(Math.random() * alpha.length)];

            this.cd = a + b + c + d;
            var refToThis = this;
            this.$emit("createcaptcha", refToThis.cd);

            var c = document.getElementById("CapCode"),
                ctx = c.getContext("2d"),
                x = c.width / 2,
                img = new Image();


            img.src = "../assets/img/captha3.jpg";
            img.onload = function () {
                var pattern = ctx.createPattern(img, "repeat");
                ctx.fillStyle = pattern;
                ctx.fillRect(0, 0, c.width, c.height);
                ctx.font = "28px Roboto Slab";
                ctx.fillStyle = '#fff';
                ctx.textAlign = 'center';
                ctx.setTransform(1, -0.12, 0, 1, 0, 15);
                ctx.fillText(refToThis.cd, x, 28);
            }

        },
        UpdateCaptcha: function () {
            this.CreateCaptcha();
        },
        ValidateCaptcha: function () {
            var refToThis = this;
            if (refToThis.EnteredValue) {


                var string1 = refToThis.cd.replace(/\s/g, '');
                var string2 = refToThis.EnteredValue;
                if (string1 == string2) {
                    this.ValidationSection = true;
                    this.success = true;
                } else {
                    this.ValidationSection = false;
                    this.success = false;
                }
            } else {
                this.ValidationSection = false;
            }
            this.$emit('validateresult', refToThis.ValidationSection);

        },
        reverseMessage: function (value) {
            return value.split('').reverse().join('');
        }
    },
    mounted: function () {
        this.cd = null;
        this.CreateCaptcha();
    },
    template: Childcomp
});