using System.Collections.Generic;
using HUFEXT.CountryCode.Runtime.API;
using HUFEXT.GenericGDPR.Runtime.API;

namespace HUFEXT.GenericGDPR.Runtime.Utils
{
    public static class GDPRTranslationsProvider
    {
        public struct GDPRTranslation
        {
            public string lang;
            public string header;
            public string policy;
            public string footer;
            public string toggle;
            public string button;
        }

        const string AD_PARTNERS_LINK = "\"https://huuugegames.com/privacy-policy/#huuuge%27s+partner+list\"";
        const string PRIVACY_POLICY_LINK = "\"https://huuugegames.com/privacy-policy\"";
        const string TERMS_OF_USE_LINK = "\"https://www.huuugegames.com/terms-of-use\"";
        
        // First element in list is default translation.
        static readonly List<GDPRTranslation> translations = new List<GDPRTranslation>
        {
            new GDPRTranslation
            {
                lang = "en",
                header = "IMPORTANT NOTICE",
                policy = "I hereby consent to the usage and disclosure of my personal data (including device information and my preferences) to <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>advertising network companies</u></color></link> for the purpose of serving targeted advertisements to me in the game. I understand that I can withdraw this consent at any time within the game Settings, as also described in our <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Privacy Policy</u></color></link>.",
                footer = "By clicking OK above, you agree to be bound by our <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Terms of Use</u></color></link> and our <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Privacy Policy</u></color></link> and you affirm that you are older than 16.",
                toggle = "I Accept",
                button = "OK"
            },

            new GDPRTranslation
            {
                lang = "pl",
                header = "Ważna Informacja",
                policy = "Wyrażam zgodę na używanie i ujawnianie moich danych osobowych (w tym informacje o urządzeniu i moich preferencjach) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>sieciom reklamowym</u></color></link> w celu dostarczania mi dopasowanych reklam w grze. Rozumiem, że mogę wycofać swoją zgodę w dowolnej chwili w Ustawieniach gry, jak wskazano też w <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Polityce Prywatności</u></color></link>.",
                footer = "Klikając w OK powyżej, akceptujesz nasze <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Warunki Korzystania</u></color></link> oraz <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Politykę Prywatności</u></color></link> oraz potwierdzasz ukończenie 16-ego roku życia.",
                toggle = "Wyrażam zgodę",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "it",
                header = "Avviso Importante",
                policy = @"Acconsento all'utilizzo e alla divulgazione dei miei dati personali (comprese le informazioni sul dispositivo e le mie preferenze) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>alle società della rete pubblicitaria</u></color></link> allo scopo di offrirmi pubblicità mirate nel gioco. Comprendo che posso revocare questo consenso in qualsiasi momento nelle Impostazioni del gioco, come descritto anche nella nostra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Informativa sulla privacy</u></color></link>.",
                footer = @"Facendo clic su OK qui sopra, l'utente accetta di essere vincolato dalle nostre <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Condizioni d'Uso</u></color></link> e dalla nostra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Informativa sulla Privacy</u></color></link> e afferma di avere più di 16 anni.",
                toggle = "Acepto",
                button = "Okay"
            },
            
            new GDPRTranslation()
            {
                lang = "es",
                header = "Noticia importante",
                policy = @"Por la presente, doy mi consentimiento para el uso y divulgación de mis datos personales (incluida la información del dispositivo y mis preferencias) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>a las empresas de publicidad</u></color></link> para recibir publicidad específica acorde a mis preferencias en el juego. Estoy informado de que puedo retirar este consentimiento en cualquier momento en el apartado \"Configuración\" del juego, como, del mismo modo, se expresa en nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de privacidad</u></color></link>.",
                footer = @"Al hacer clic en Aceptar arriba, se acepta estar sujeto a nuestros <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Términos de uso</u></color></link> y condiciones y nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de privacidad</u></color></link> y afirma que es mayor de 16 años.",
                toggle = "Acepto",
                button = "Okay"
            },
            
            new GDPRTranslation()
            {
                lang = "es-mx",
                header = "Aviso importante",
                policy = @"Por la presente doy mi consentimiento para el uso y la divulgación de mis datos personales (incluida la información del dispositivo y mis preferencias) a las <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>empresas de publicidad</u></color></link>, con el fin de que me muestren publicidad personalizada en el juego. Entiendo que puedo revocar este consentimiento en cualquier momento dentro de las Configuraciones del juego, como también se describe en nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de privacidad</u></color></link>.",
                footer = @"Al hacer clic en Aceptar, accede a estar sujeto a nuestras <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Condiciones de uso</u></color></link> y a nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de privacidad</u></color></link> y afirma que es mayor de 16 años.",
                toggle = "Aceptar",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "ja",
                header = "重要なお知らせ",
                policy = @"本サービスを利用されたことに伴い取得したユーザーの情報、デバイス情報及びユーザーに合わせた広告その他の情報を、ユーザーの事前の同意なく<link=" + AD_PARTNERS_LINK + "><color=#{0}><u>第三者</u></color></link>へ提供できるものとします。<link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>プライバシーポリシー</u></color></link>に記載されているようにユーザーはいつでも同意を取り消すことができます。",
                footer = @"本<link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>規約</u></color></link>及び<link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>プライバシーポリシー</u></color></link>をお読みいただき、「同意する」ボタンを押してください。（同意した方は16歳以上と判断させていただきます）",
                toggle = "同意する",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "fr",
                header = "Avis important",
                policy = @"Je consens par la présente à l'utilisation et à la divulgation de mes données personnelles (y compris les informations relatives à l'appareil et mes préférences) aux <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>sociétés de réseaux publicitaires</u></color></link> dans le but de me faire parvenir des publicités ciblées dans le jeu. Je peux retirer ce consentement à tout moment dans les paramètres du jeu, comme indiqué dans la <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>politique de confidentialité</u></color></link>.",
                footer = @"En cliquant sur OK ci-dessus, vous acceptez nos <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>conditions d'utilisation</u></color></link>  et notre <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>politique de confidentialité</u></color></link> et vous affirmez avoir plus de 16 ans.",
                toggle = "J'accepte",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "de",
                header = "Wichtiger Hinweis",
                policy = @"Ich stimme hiermit der Verwendung und Weitergabe meiner persönlichen Daten (einschließlich Geräteinformationen und meiner Präferenzen) an <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>Werbenetzwerkunternehmen</u></color></link> zu, um mir im Spiel gezielte Werbung zu liefern. Ich habe verstanden, dass ich diese Einwilligung, wie auch in unserer <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Datenschutzrichtlinie</u></color></link> beschrieben, jederzeit innerhalb der Spieleinstellungen widerrufen kann.",
                footer = @"Wenn du oben auf OK klickst, erklärst du dich mit unseren <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Nutzungsbedingungen</u></color></link> und unserer <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Datenschutzrichtlinie</u></color></link> einverstanden und bestätigst, dass du älter als 16 Jahre bist.",
                toggle = "Ich nehme an",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "ko",
                header = "중요 참고사항",
                policy= @"본인은 이로써 게임에서 본인에게 표적 광고를 제공할 목적을 위해 <link=" + AD_PARTNERS_LINK + @"><color=#{0}><u>광고 네트워크 회사</u></color></link>에 내 개인 데이터(장치 정보 및 본인의 선호 포함)를 사용하고 공개하는 데 동의합니다. 본인은 우리의 <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>개인정보보호 정책</u></color></link>에서도 기술된 바와 같이 게임 설정 내에서 언제든지 이 동의를 철회할 수 있습니다.",
                footer = @"위의 확인을 클릭함으로써 귀하는 당사의 <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>사용 약관</u></color></link> 및 당사의 <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>개인정보보호 정책</u></color></link>의 적용을 받는 데 동의하게 되며 귀하가 16세를 초과한다는 것을 확인하게 됩니다.",
                toggle = "동의합니다.",
                button = "확인"
            },
            
            new GDPRTranslation()
            {
                lang = "pt",
                header = "Aviso importante",
                policy = @"Concordo pelo presente que o uso e divulgação de meus dados pessoais (incluindo informações de dispositivo e minhas preferências) para <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>empresas de rede de publicidade</u></color></link> para o propósito de servir anúncios segmentados para mim no jogo. Entendo que eu posso retirar este consentimento a qualquer hora nas Definições de jogo, como também descrito na nossa <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de Privacidade</u></color></link>.",
                footer = @"Ao clicar em OK acima, você concorda em estar ligado aos nossos <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Termos de Uso</u></color></link> e a nossa <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de Privacidade</u></color></link> e você afirma que você tem mais de 16 anos.",
                toggle = "Eu aceito",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "pt-br",
                header = "Aviso Importante",
                policy = @"Dou permissão para usar e divulgar meus dados pessoais (incluindo informações do dispositivo e preferências) para <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>empresas de marketing de rede</u></color></link> com o propósito de receber anúncios personalizados no jogo. Eu compreendo que posso retirar esta permissão a qualquer momento nas configurações do jogo, como também está descrito na <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de Privacidade</u></color></link>.",
                footer = @"Ao clicar em OK acima, você concorda com o vínculo dos nossos <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Termos de Uso</u></color></link> e nossa <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de Privacidade</u></color></link>, e confirma ter mais de 16 anos.",
                toggle = "Aceito",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "ru",
                header = "Важная информация",
                policy = @"Настоящим я даю согласие на использование и разглашение моих персональных данных (включая информацию об устройстве и о моих предпочтениях) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>сетевым рекламным компаниям</u></color></link> с целью подачи мне адресной рекламы в процессе игры. Я понимаю, что могу отозвать это свое согласие в любое время в Настройках игры, что также описано в  <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Политике Конфиденциальности</u></color></link>.",
                footer = @"Нажимая OK выше, ты соглашаешься с нашими <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>Условиями использования и</u></color></link> нашей <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Политикой Конфиденциальности</u></color></link>, и подтверждаешь, что тебе больше 16 лет.",
                toggle = "Я принимаю",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "nb",
                header = "Viktig merknad",
                policy = @"Med dette gir du samtykke til at dine personopplysninger (deriblant informasjon om enheten din og dine preferanser) kan brukes av og deles med <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>selskaper som driver med annonsenettverk</u></color></link> med formål om å levere målrettet reklame til deg i spillet. Du forstår at du når som helst kan trekke tilbake dette samtykket fra innstillingene i spillet, noe som også er beskrevet i vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>personvernerklæring</u></color></link>.",
                footer = @"Ved å klikke på OK ovenfor godtar du å være bundet av våre <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>bruksvilkår</u></color></link> og vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>personvernerklæring</u></color></link>, og du bekrefter at du er over 16 år.",
                toggle = "Jeg godtar",
                button = "OK"
            },
            
            new GDPRTranslation()
            {
                lang = "sv",
                header = "Viktig information",
                policy = @"Härmed går jag med på att mina personuppgifter (inklusive enhetsinformation och mina inställningar) används och delas med <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>reklamnätverk</u></color></link> i syfte att ge mig riktad reklam i spelet. Jag är medveten om att jag kan dra tillbaka mitt samtycke när som helst i spelets inställningar, enligt vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>integritetspolicy</u></color></link>.",
                footer = @"Genom att klicka på OK ovan går du med på att följa våra <link=" + TERMS_OF_USE_LINK + "><color=#{0}><u>användarvillkor</u></color></link> och vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>integritetspolicy</u></color></link>, samt bekräftar att du har fyllt 16 år.",
                toggle = "Jag godkänner",
                button = "OK"
            }
        };
        
        public static GDPRTranslation DefaultTranslation => translations[0];

        public static List<GDPRTranslation> Translations => translations;
        
        public static GDPRTranslation GetTranslation( GDPRConfig config )
        {
            if ( config == null || !config.IsTranslationEnabled )
            {
                return translations[0];
            }
            
            var lang = HNativeCountryCode.GetCountryCode().Language;
            var code = HNativeCountryCode.GetCountryCode().GetCountryCode();

            // Check by full country code {xx-yy}.
            for ( int i = 0; i < translations.Count; ++i )
            {
                if ( translations[i].lang == code )
                {
                    return translations[i];
                }
            }
            
            // Check only via language code {xx}.
            for ( int i = 0; i < translations.Count; ++i )
            {
                if ( translations[i].lang == lang )
                {
                    return translations[i];
                }
            }

            return translations[0];
        }
    }
}