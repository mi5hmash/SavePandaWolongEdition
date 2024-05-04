using SavePandaWolongEditionCore.Helpers;

namespace SavePandaWLE.Helpers
{
    public class SimpleMediatorWinForms : ISimpleMediator
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SimpleMediatorWinForms() { }

        /// <summary>
        /// Translates <see cref="ISimpleMediator.QuestionOptions"/> to <see cref="MessageBoxButtons"/>
        /// </summary>
        /// <param name="questionOptions"></param>
        /// <returns></returns>
        private static MessageBoxButtons GetDialogOpt(ISimpleMediator.QuestionOptions questionOptions)
        {
            return questionOptions switch
            {
                ISimpleMediator.QuestionOptions.OkCancel => MessageBoxButtons.OKCancel,
                ISimpleMediator.QuestionOptions.AbortRetryIgnore => MessageBoxButtons.AbortRetryIgnore,
                ISimpleMediator.QuestionOptions.YesNoCancel => MessageBoxButtons.YesNoCancel,
                ISimpleMediator.QuestionOptions.YesNo => MessageBoxButtons.YesNo,
                ISimpleMediator.QuestionOptions.RetryCancel => MessageBoxButtons.RetryCancel,
                ISimpleMediator.QuestionOptions.CancelTryContinue => MessageBoxButtons.CancelTryContinue,
                _ => MessageBoxButtons.OKCancel
            };
        }

        /// <summary>
        /// Translates <see cref="ISimpleMediator.DialogType"/> to <see cref="MessageBoxIcon"/>
        /// </summary>
        /// <param name="dialogType"></param>
        /// <returns></returns>
        private static MessageBoxIcon GetDialogType(ISimpleMediator.DialogType dialogType)
        {
            return dialogType switch
            {
                ISimpleMediator.DialogType.None => MessageBoxIcon.None,
                ISimpleMediator.DialogType.Question => MessageBoxIcon.Question,
                ISimpleMediator.DialogType.Exclamation => MessageBoxIcon.Exclamation,
                ISimpleMediator.DialogType.Error => MessageBoxIcon.Error,
                ISimpleMediator.DialogType.Warning => MessageBoxIcon.Warning,
                ISimpleMediator.DialogType.Information => MessageBoxIcon.Information,
                _ => MessageBoxIcon.None
            };
        }

        /// <summary>
        /// Ask the user a simple question and get an answer.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="caption"></param>
        /// <param name="questionOptions"></param>
        /// <param name="dialogType"></param>
        /// <returns></returns>
        public ISimpleMediator.DialogAnswer Ask(string question, string caption, ISimpleMediator.QuestionOptions questionOptions, ISimpleMediator.DialogType dialogType)
        {
            var dlgOpt = GetDialogOpt(questionOptions);
            var dlgType = GetDialogType(dialogType);

            var dialogResult = MessageBox.Show(question, caption, dlgOpt, dlgType);

            return dialogResult switch
            {
                DialogResult.None => ISimpleMediator.DialogAnswer.None,
                DialogResult.OK => ISimpleMediator.DialogAnswer.Ok,
                DialogResult.Cancel => ISimpleMediator.DialogAnswer.Cancel,
                DialogResult.Abort => ISimpleMediator.DialogAnswer.Abort,
                DialogResult.Retry => ISimpleMediator.DialogAnswer.Retry,
                DialogResult.Ignore => ISimpleMediator.DialogAnswer.Ignore,
                DialogResult.Yes => ISimpleMediator.DialogAnswer.Yes,
                DialogResult.No => ISimpleMediator.DialogAnswer.No,
                DialogResult.TryAgain => ISimpleMediator.DialogAnswer.TryAgain,
                DialogResult.Continue => ISimpleMediator.DialogAnswer.Continue,
                _ => ISimpleMediator.DialogAnswer.None
            };
        }

        /// <summary>
        /// Send a message to the user.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="caption"></param>
        /// <param name="dialogType"></param>
        public void Inform(string info, string caption, ISimpleMediator.DialogType dialogType)
        {
            var dlgType = GetDialogType(dialogType);
            MessageBox.Show(info, caption, MessageBoxButtons.OK, dlgType);
        }
    }
}
