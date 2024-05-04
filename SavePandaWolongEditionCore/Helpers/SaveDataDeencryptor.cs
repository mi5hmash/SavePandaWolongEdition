using System.Runtime.InteropServices;

namespace SavePandaWolongEditionCore.Helpers;

public class SaveDataDeencryptor
{
    #region CONSTANTS

    private const int ContainerLength = 4;
    private const int ContainerLengthBytes = 16;
    private readonly int[] _sequence = [1, 2, 3, 0, 1, 2, 3];

    private readonly uint[] _privateKey;

    private readonly byte[] _hashTableE0;
    private readonly byte[] _hashTableD0;
    private readonly byte[] _hashTableE1;
    private readonly byte[] _hashTableD1;

    private readonly byte[] _hashTableC;

    #endregion

    /// <summary>
    /// Default Constructor that loads configuration.
    /// </summary>
    public SaveDataDeencryptor()
    {
        _privateKey = "QkI0MDg0MEE1RjlDN0Q0MEMxOEU5MDI4RkJFRTUxOEY="
            .Base64DecodeUtf8().ToUintArray();

        _hashTableE0 = "MDA2MzYzQzZBNTYzNjNDNjAwN0M3Q0Y4ODQ3QzdDRjgwMDc3NzdFRTk5Nzc3N0VFMDA3QjdCRjY4RDdCN0JGNjAwRjJGMkZGMERGMkYyRkYwMDZCNkJENkJENkI2QkQ2MDA2RjZGREVCMTZGNkZERTAwQzVDNTkxNTRDNUM1OTEwMDMwMzA2MDUwMzAzMDYwMDAwMTAxMDIwMzAxMDEwMjAwNjc2N0NFQTk2NzY3Q0UwMDJCMkI1NjdEMkIyQjU2MDBGRUZFRTcxOUZFRkVFNzAwRDdEN0I1NjJEN0Q3QjUwMEFCQUI0REU2QUJBQjREMDA3Njc2RUM5QTc2NzZFQzAwQ0FDQThGNDVDQUNBOEYwMDgyODIxRjlEODI4MjFGMDBDOUM5ODk0MEM5Qzk4OTAwN0Q3REZBODc3RDdERkEwMEZBRkFFRjE1RkFGQUVGMDA1OTU5QjJFQjU5NTlCMjAwNDc0NzhFQzk0NzQ3OEUwMEYwRjBGQjBCRjBGMEZCMDBBREFENDFFQ0FEQUQ0MTAwRDRENEIzNjdENEQ0QjMwMEEyQTI1RkZEQTJBMjVGMDBBRkFGNDVFQUFGQUY0NTAwOUM5QzIzQkY5QzlDMjMwMEE0QTQ1M0Y3QTRBNDUzMDA3MjcyRTQ5NjcyNzJFNDAwQzBDMDlCNUJDMEMwOUIwMEI3Qjc3NUMyQjdCNzc1MDBGREZERTExQ0ZERkRFMTAwOTM5MzNEQUU5MzkzM0QwMDI2MjY0QzZBMjYyNjRDMDAzNjM2NkM1QTM2MzY2QzAwM0YzRjdFNDEzRjNGN0UwMEY3RjdGNTAyRjdGN0Y1MDBDQ0NDODM0RkNDQ0M4MzAwMzQzNDY4NUMzNDM0NjgwMEE1QTU1MUY0QTVBNTUxMDBFNUU1RDEzNEU1RTVEMTAwRjFGMUY5MDhGMUYxRjkwMDcxNzFFMjkzNzE3MUUyMDBEOEQ4QUI3M0Q4RDhBQjAwMzEzMTYyNTMzMTMxNjIwMDE1MTUyQTNGMTUxNTJBMDAwNDA0MDgwQzA0MDQwODAwQzdDNzk1NTJDN0M3OTUwMDIzMjM0NjY1MjMyMzQ2MDBDM0MzOUQ1RUMzQzM5RDAwMTgxODMwMjgxODE4MzAwMDk2OTYzN0ExOTY5NjM3MDAwNTA1MEEwRjA1MDUwQTAwOUE5QTJGQjU5QTlBMkYwMDA3MDcwRTA5MDcwNzBFMDAxMjEyMjQzNjEyMTIyNDAwODA4MDFCOUI4MDgwMUIwMEUyRTJERjNERTJFMkRGMDBFQkVCQ0QyNkVCRUJDRDAwMjcyNzRFNjkyNzI3NEUwMEIyQjI3RkNEQjJCMjdGMDA3NTc1RUE5Rjc1NzVFQTAwMDkwOTEyMUIwOTA5MTIwMDgzODMxRDlFODM4MzFEMDAyQzJDNTg3NDJDMkM1ODAwMUExQTM0MkUxQTFBMzQwMDFCMUIzNjJEMUIxQjM2MDA2RTZFRENCMjZFNkVEQzAwNUE1QUI0RUU1QTVBQjQwMEEwQTA1QkZCQTBBMDVCMDA1MjUyQTRGNjUyNTJBNDAwM0IzQjc2NEQzQjNCNzYwMEQ2RDZCNzYxRDZENkI3MDBCM0IzN0RDRUIzQjM3RDAwMjkyOTUyN0IyOTI5NTIwMEUzRTNERDNFRTNFM0REMDAyRjJGNUU3MTJGMkY1RTAwODQ4NDEzOTc4NDg0MTMwMDUzNTNBNkY1NTM1M0E2MDBEMUQxQjk2OEQxRDFCOTAwMDAwMDAwMDAwMDAwMDAwMEVERURDMTJDRURFREMxMDAyMDIwNDA2MDIwMjA0MDAwRkNGQ0UzMUZGQ0ZDRTMwMEIxQjE3OUM4QjFCMTc5MDA1QjVCQjZFRDVCNUJCNjAwNkE2QUQ0QkU2QTZBRDQwMENCQ0I4RDQ2Q0JDQjhEMDBCRUJFNjdEOUJFQkU2NzAwMzkzOTcyNEIzOTM5NzIwMDRBNEE5NERFNEE0QTk0MDA0QzRDOThENDRDNEM5ODAwNTg1OEIwRTg1ODU4QjAwMENGQ0Y4NTRBQ0ZDRjg1MDBEMEQwQkI2QkQwRDBCQjAwRUZFRkM1MkFFRkVGQzUwMEFBQUE0RkU1QUFBQTRGMDBGQkZCRUQxNkZCRkJFRDAwNDM0Mzg2QzU0MzQzODYwMDRENEQ5QUQ3NEQ0RDlBMDAzMzMzNjY1NTMzMzM2NjAwODU4NTExOTQ4NTg1MTEwMDQ1NDU4QUNGNDU0NThBMDBGOUY5RTkxMEY5RjlFOTAwMDIwMjA0MDYwMjAyMDQwMDdGN0ZGRTgxN0Y3RkZFMDA1MDUwQTBGMDUwNTBBMDAwM0MzQzc4NDQzQzNDNzgwMDlGOUYyNUJBOUY5RjI1MDBBOEE4NEJFM0E4QTg0QjAwNTE1MUEyRjM1MTUxQTIwMEEzQTM1REZFQTNBMzVEMDA0MDQwODBDMDQwNDA4MDAwOEY4RjA1OEE4RjhGMDUwMDkyOTIzRkFEOTI5MjNGMDA5RDlEMjFCQzlEOUQyMTAwMzgzODcwNDgzODM4NzAwMEY1RjVGMTA0RjVGNUYxMDBCQ0JDNjNERkJDQkM2MzAwQjZCNjc3QzFCNkI2NzcwMERBREFBRjc1REFEQUFGMDAyMTIxNDI2MzIxMjE0MjAwMTAxMDIwMzAxMDEwMjAwMEZGRkZFNTFBRkZGRkU1MDBGM0YzRkQwRUYzRjNGRDAwRDJEMkJGNkREMkQyQkYwMENEQ0Q4MTRDQ0RDRDgxMDAwQzBDMTgxNDBDMEMxODAwMTMxMzI2MzUxMzEzMjYwMEVDRUNDMzJGRUNFQ0MzMDA1RjVGQkVFMTVGNUZCRTAwOTc5NzM1QTI5Nzk3MzUwMDQ0NDQ4OENDNDQ0NDg4MDAxNzE3MkUzOTE3MTcyRTAwQzRDNDkzNTdDNEM0OTMwMEE3QTc1NUYyQTdBNzU1MDA3RTdFRkM4MjdFN0VGQzAwM0QzRDdBNDczRDNEN0EwMDY0NjRDOEFDNjQ2NEM4MDA1RDVEQkFFNzVENURCQTAwMTkxOTMyMkIxOTE5MzIwMDczNzNFNjk1NzM3M0U2MDA2MDYwQzBBMDYwNjBDMDAwODE4MTE5OTg4MTgxMTkwMDRGNEY5RUQxNEY0RjlFMDBEQ0RDQTM3RkRDRENBMzAwMjIyMjQ0NjYyMjIyNDQwMDJBMkE1NDdFMkEyQTU0MDA5MDkwM0JBQjkwOTAzQjAwODg4ODBCODM4ODg4MEIwMDQ2NDY4Q0NBNDY0NjhDMDBFRUVFQzcyOUVFRUVDNzAwQjhCODZCRDNCOEI4NkIwMDE0MTQyODNDMTQxNDI4MDBERURFQTc3OURFREVBNzAwNUU1RUJDRTI1RTVFQkMwMDBCMEIxNjFEMEIwQjE2MDBEQkRCQUQ3NkRCREJBRDAwRTBFMERCM0JFMEUwREIwMDMyMzI2NDU2MzIzMjY0MDAzQTNBNzQ0RTNBM0E3NDAwMEEwQTE0MUUwQTBBMTQwMDQ5NDk5MkRCNDk0OTkyMDAwNjA2MEMwQTA2MDYwQzAwMjQyNDQ4NkMyNDI0NDgwMDVDNUNCOEU0NUM1Q0I4MDBDMkMyOUY1REMyQzI5RjAwRDNEM0JENkVEM0QzQkQwMEFDQUM0M0VGQUNBQzQzMDA2MjYyQzRBNjYyNjJDNDAwOTE5MTM5QTg5MTkxMzkwMDk1OTUzMUE0OTU5NTMxMDBFNEU0RDMzN0U0RTREMzAwNzk3OUYyOEI3OTc5RjIwMEU3RTdENTMyRTdFN0Q1MDBDOEM4OEI0M0M4Qzg4QjAwMzczNzZFNTkzNzM3NkUwMDZENkREQUI3NkQ2RERBMDA4RDhEMDE4QzhEOEQwMTAwRDVENUIxNjRENUQ1QjEwMDRFNEU5Q0QyNEU0RTlDMDBBOUE5NDlFMEE5QTk0OTAwNkM2Q0Q4QjQ2QzZDRDgwMDU2NTZBQ0ZBNTY1NkFDMDBGNEY0RjMwN0Y0RjRGMzAwRUFFQUNGMjVFQUVBQ0YwMDY1NjVDQUFGNjU2NUNBMDA3QTdBRjQ4RTdBN0FGNDAwQUVBRTQ3RTlBRUFFNDcwMDA4MDgxMDE4MDgwODEwMDBCQUJBNkZENUJBQkE2RjAwNzg3OEYwODg3ODc4RjAwMDI1MjU0QTZGMjUyNTRBMDAyRTJFNUM3MjJFMkU1QzAwMUMxQzM4MjQxQzFDMzgwMEE2QTY1N0YxQTZBNjU3MDBCNEI0NzNDN0I0QjQ3MzAwQzZDNjk3NTFDNkM2OTcwMEU4RThDQjIzRThFOENCMDBEREREQTE3Q0RERERBMTAwNzQ3NEU4OUM3NDc0RTgwMDFGMUYzRTIxMUYxRjNFMDA0QjRCOTZERDRCNEI5NjAwQkRCRDYxRENCREJENjEwMDhCOEIwRDg2OEI4QjBEMDA4QThBMEY4NThBOEEwRjAwNzA3MEUwOTA3MDcwRTAwMDNFM0U3QzQyM0UzRTdDMDBCNUI1NzFDNEI1QjU3MTAwNjY2NkNDQUE2NjY2Q0MwMDQ4NDg5MEQ4NDg0ODkwMDAwMzAzMDYwNTAzMDMwNjAwRjZGNkY3MDFGNkY2RjcwMDBFMEUxQzEyMEUwRTFDMDA2MTYxQzJBMzYxNjFDMjAwMzUzNTZBNUYzNTM1NkEwMDU3NTdBRUY5NTc1N0FFMDBCOUI5NjlEMEI5Qjk2OTAwODY4NjE3OTE4Njg2MTcwMEMxQzE5OTU4QzFDMTk5MDAxRDFEM0EyNzFEMUQzQTAwOUU5RTI3Qjk5RTlFMjcwMEUxRTFEOTM4RTFFMUQ5MDBGOEY4RUIxM0Y4RjhFQjAwOTg5ODJCQjM5ODk4MkIwMDExMTEyMjMzMTExMTIyMDA2OTY5RDJCQjY5NjlEMjAwRDlEOUE5NzBEOUQ5QTkwMDhFOEUwNzg5OEU4RTA3MDA5NDk0MzNBNzk0OTQzMzAwOUI5QjJEQjY5QjlCMkQwMDFFMUUzQzIyMUUxRTNDMDA4Nzg3MTU5Mjg3ODcxNTAwRTlFOUM5MjBFOUU5QzkwMENFQ0U4NzQ5Q0VDRTg3MDA1NTU1QUFGRjU1NTVBQTAwMjgyODUwNzgyODI4NTAwMERGREZBNTdBREZERkE1MDA4QzhDMDM4RjhDOEMwMzAwQTFBMTU5RjhBMUExNTkwMDg5ODkwOTgwODk4OTA5MDAwRDBEMUExNzBEMEQxQTAwQkZCRjY1REFCRkJGNjUwMEU2RTZENzMxRTZFNkQ3MDA0MjQyODRDNjQyNDI4NDAwNjg2OEQwQjg2ODY4RDAwMDQxNDE4MkMzNDE0MTgyMDA5OTk5MjlCMDk5OTkyOTAwMkQyRDVBNzcyRDJENUEwMDBGMEYxRTExMEYwRjFFMDBCMEIwN0JDQkIwQjA3QjAwNTQ1NEE4RkM1NDU0QTgwMEJCQkI2REQ2QkJCQjZEMDAxNjE2MkMzQTE2MTYyQw=="
            .Base64DecodeUtf8().ToBytes();
        _hashTableD0 = "NTJBN0Y0NTE1MEE3RjQ1MTA5NjU0MTdFNTM2NTQxN0U2QUE0MTcxQUMzQTQxNzFBRDU1RTI3M0E5NjVFMjczQTMwNkJBQjNCQ0I2QkFCM0IzNjQ1OUQxRkYxNDU5RDFGQTU1OEZBQUNBQjU4RkFBQzM4MDNFMzRCOTMwM0UzNEJCRkZBMzAyMDU1RkEzMDIwNDA2RDc2QURGNjZENzZBREEzNzZDQzg4OTE3NkNDODg5RTRDMDJGNTI1NEMwMkY1ODFEN0U1NEZGQ0Q3RTU0RkYzQ0IyQUM1RDdDQjJBQzVENzQ0MzUyNjgwNDQzNTI2RkJBMzYyQjU4RkEzNjJCNTdDNUFCMURFNDk1QUIxREVFMzFCQkEyNTY3MUJCQTI1MzkwRUVBNDU5ODBFRUE0NTgyQzBGRTVERTFDMEZFNUQ5Qjc1MkZDMzAyNzUyRkMzMkZGMDRDODExMkYwNEM4MUZGOTc0NjhEQTM5NzQ2OEQ4N0Y5RDM2QkM2RjlEMzZCMzQ1RjhGMDNFNzVGOEYwMzhFOUM5MjE1OTU5QzkyMTU0MzdBNkRCRkVCN0E2REJGNDQ1OTUyOTVEQTU5NTI5NUM0ODNCRUQ0MkQ4M0JFRDRERTIxNzQ1OEQzMjE3NDU4RTk2OUUwNDkyOTY5RTA0OUNCQzhDOThFNDRDOEM5OEU1NDg5QzI3NTZBODlDMjc1N0I3OThFRjQ3ODc5OEVGNDk0M0U1ODk5NkIzRTU4OTkzMjcxQjkyN0RENzFCOTI3QTY0RkUxQkVCNjRGRTFCRUMyQUQ4OEYwMTdBRDg4RjAyM0FDMjBDOTY2QUMyMEM5M0QzQUNFN0RCNDNBQ0U3REVFNEFERjYzMTg0QURGNjM0QzMxMUFFNTgyMzExQUU1OTUzMzUxOTc2MDMzNTE5NzBCN0Y1MzYyNDU3RjUzNjI0Mjc3NjRCMUUwNzc2NEIxRkFBRTZCQkI4NEFFNkJCQkMzQTA4MUZFMUNBMDgxRkU0RTJCMDhGOTk0MkIwOEY5MDg2ODQ4NzA1ODY4NDg3MDJFRkQ0NThGMTlGRDQ1OEZBMTZDREU5NDg3NkNERTk0NjZGODdCNTJCN0Y4N0I1MjI4RDM3M0FCMjNEMzczQUJEOTAyNEI3MkUyMDI0QjcyMjQ4RjFGRTM1NzhGMUZFM0IyQUI1NTY2MkFBQjU1NjY3NjI4RUJCMjA3MjhFQkIyNUJDMkI1MkYwM0MyQjUyRkEyN0JDNTg2OUE3QkM1ODY0OTA4MzdEM0E1MDgzN0QzNkQ4NzI4MzBGMjg3MjgzMDhCQTVCRjIzQjJBNUJGMjNEMTZBMDMwMkJBNkEwMzAyMjU4MjE2RUQ1QzgyMTZFRDcyMUNDRjhBMkIxQ0NGOEFGOEI0NzlBNzkyQjQ3OUE3RjZGMjA3RjNGMEYyMDdGMzY0RTI2OTRFQTFFMjY5NEU4NkY0REE2NUNERjREQTY1NjhCRTA1MDZENUJFMDUwNjk4NjIzNEQxMUY2MjM0RDExNkZFQTZDNDhBRkVBNkM0RDQ1MzJFMzQ5RDUzMkUzNEE0NTVGM0EyQTA1NUYzQTI1Q0UxOEEwNTMyRTE4QTA1Q0NFQkY2QTQ3NUVCRjZBNDVERUM4MzBCMzlFQzgzMEI2NUVGNjA0MEFBRUY2MDQwQjY5RjcxNUUwNjlGNzE1RTkyMTA2RUJENTExMDZFQkQ2QzhBMjEzRUY5OEEyMTNFNzAwNkREOTYzRDA2REQ5NjQ4MDUzRUREQUUwNTNFREQ1MEJERTY0RDQ2QkRFNjRERkQ4RDU0OTFCNThENTQ5MUVENURDNDcxMDU1REM0NzFCOUQ0MDYwNDZGRDQwNjA0REExNTUwNjBGRjE1NTA2MDVFRkI5ODE5MjRGQjk4MTkxNUU5QkRENjk3RTlCREQ2NDY0MzQwODlDQzQzNDA4OTU3OUVEOTY3Nzc5RUQ5NjdBNzQyRThCMEJENDJFOEIwOEQ4Qjg5MDc4ODhCODkwNzlENUIxOUU3Mzg1QjE5RTc4NEVFQzg3OURCRUVDODc5OTAwQTdDQTE0NzBBN0NBMUQ4MEY0MjdDRTkwRjQyN0NBQjFFODRGOEM5MUU4NEY4MDAwMDAwMDAwMDAwMDAwMDhDODY4MDA5ODM4NjgwMDlCQ0VEMkIzMjQ4RUQyQjMyRDM3MDExMUVBQzcwMTExRTBBNzI1QTZDNEU3MjVBNkNGN0ZGMEVGREZCRkYwRUZERTQzODg1MEY1NjM4ODUwRjU4RDVBRTNEMUVENUFFM0QwNTM5MkQzNjI3MzkyRDM2QjhEOTBGMEE2NEQ5MEYwQUIzQTY1QzY4MjFBNjVDNjg0NTU0NUI5QkQxNTQ1QjlCMDYyRTM2MjQzQTJFMzYyNEQwNjcwQTBDQjE2NzBBMEMyQ0U3NTc5MzBGRTc1NzkzMUU5NkVFQjREMjk2RUVCNDhGOTE5QjFCOUU5MTlCMUJDQUM1QzA4MDRGQzVDMDgwM0YyMERDNjFBMjIwREM2MTBGNEI3NzVBNjk0Qjc3NUEwMjFBMTIxQzE2MUExMjFDQzFCQTkzRTIwQUJBOTNFMkFGMkFBMEMwRTUyQUEwQzBCREUwMjIzQzQzRTAyMjNDMDMxNzFCMTIxRDE3MUIxMjAxMEQwOTBFMEIwRDA5MEUxM0M3OEJGMkFEQzc4QkYyOEFBOEI2MkRCOUE4QjYyRDZCQTkxRTE0QzhBOTFFMTQzQTE5RjE1Nzg1MTlGMTU3OTEwNzc1QUY0QzA3NzVBRjExREQ5OUVFQkJERDk5RUU0MTYwN0ZBM0ZENjA3RkEzNEYyNjAxRjc5RjI2MDFGNzY3RjU3MjVDQkNGNTcyNUNEQzNCNjY0NEM1M0I2NjQ0RUE3RUZCNUIzNDdFRkI1Qjk3Mjk0MzhCNzYyOTQzOEJGMkM2MjNDQkRDQzYyM0NCQ0ZGQ0VEQjY2OEZDRURCNkNFRjFFNEI4NjNGMUU0QjhGMERDMzFEN0NBREMzMUQ3QjQ4NTYzNDIxMDg1NjM0MkU2MjI5NzEzNDAyMjk3MTM3MzExQzY4NDIwMTFDNjg0OTYyNDRBODU3RDI0NEE4NUFDM0RCQkQyRjgzREJCRDI3NDMyRjlBRTExMzJGOUFFMjJBMTI5Qzc2REExMjlDN0U3MkY5RTFENEIyRjlFMURBRDMwQjJEQ0YzMzBCMkRDMzU1Mjg2MERFQzUyODYwRDg1RTNDMTc3RDBFM0MxNzdFMjE2QjMyQjZDMTZCMzJCRjlCOTcwQTk5OUI5NzBBOTM3NDg5NDExRkE0ODk0MTFFODY0RTk0NzIyNjRFOTQ3MUM4Q0ZDQThDNDhDRkNBODc1M0ZGMEEwMUEzRkYwQTBERjJDN0Q1NkQ4MkM3RDU2NkU5MDMzMjJFRjkwMzMyMjQ3NEU0OTg3Qzc0RTQ5ODdGMUQxMzhEOUMxRDEzOEQ5MUFBMkNBOENGRUEyQ0E4QzcxMEJENDk4MzYwQkQ0OTgxRDgxRjVBNkNGODFGNUE2MjlERTdBQTUyOERFN0FBNUM1OEVCN0RBMjY4RUI3REE4OUJGQUQzRkE0QkZBRDNGNkY5RDNBMkNFNDlEM0EyQ0I3OTI3ODUwMEQ5Mjc4NTA2MkNDNUY2QTlCQ0M1RjZBMEU0NjdFNTQ2MjQ2N0U1NEFBMTM4REY2QzIxMzhERjYxOEI4RDg5MEU4QjhEODkwQkVGNzM5MkU1RUY3MzkyRTFCQUZDMzgyRjVBRkMzODJGQzgwNUQ5RkJFODA1RDlGNTY5M0QwNjk3QzkzRDA2OTNFMkRENTZGQTkyREQ1NkY0QjEyMjVDRkIzMTIyNUNGQzY5OUFDQzgzQjk5QUNDOEQyN0QxODEwQTc3RDE4MTA3OTYzOUNFODZFNjM5Q0U4MjBCQjNCREI3QkJCM0JEQjlBNzgyNkNEMDk3ODI2Q0REQjE4NTk2RUY0MTg1OTZFQzBCNzlBRUMwMUI3OUFFQ0ZFOUE0RjgzQTg5QTRGODM3ODZFOTVFNjY1NkU5NUU2Q0RFNkZGQUE3RUU2RkZBQTVBQ0ZCQzIxMDhDRkJDMjFGNEU4MTVFRkU2RTgxNUVGMUY5QkU3QkFEOTlCRTdCQUREMzY2RjRBQ0UzNjZGNEFBODA5OUZFQUQ0MDk5RkVBMzM3Q0IwMjlENjdDQjAyOTg4QjJBNDMxQUZCMkE0MzEwNzIzM0YyQTMxMjMzRjJBQzc5NEE1QzYzMDk0QTVDNjMxNjZBMjM1QzA2NkEyMzVCMUJDNEU3NDM3QkM0RTc0MTJDQTgyRkNBNkNBODJGQzEwRDA5MEUwQjBEMDkwRTA1OUQ4QTczMzE1RDhBNzMzMjc5ODA0RjE0QTk4MDRGMTgwREFFQzQxRjdEQUVDNDFFQzUwQ0Q3RjBFNTBDRDdGNUZGNjkxMTcyRkY2OTExNzYwRDY0RDc2OERENjRENzY1MUIwRUY0MzREQjBFRjQzN0Y0REFBQ0M1NDREQUFDQ0E5MDQ5NkU0REYwNDk2RTQxOUI1RDE5RUUzQjVEMTlFQjU4ODZBNEMxQjg4NkE0QzRBMUYyQ0MxQjgxRjJDQzEwRDUxNjU0NjdGNTE2NTQ2MkRFQTVFOUQwNEVBNUU5REU1MzU4QzAxNUQzNThDMDE3QTc0ODdGQTczNzQ4N0ZBOUY0MTBCRkIyRTQxMEJGQjkzMUQ2N0IzNUExRDY3QjNDOUQyREI5MjUyRDJEQjkyOUM1NjEwRTkzMzU2MTBFOUVGNDdENjZEMTM0N0Q2NkRBMDYxRDc5QThDNjFENzlBRTAwQ0ExMzc3QTBDQTEzNzNCMTRGODU5OEUxNEY4NTk0RDNDMTNFQjg5M0MxM0VCQUUyN0E5Q0VFRTI3QTlDRTJBQzk2MUI3MzVDOTYxQjdGNUU1MUNFMUVERTUxQ0UxQjBCMTQ3N0EzQ0IxNDc3QUM4REZEMjlDNTlERkQyOUNFQjczRjI1NTNGNzNGMjU1QkJDRTE0MTg3OUNFMTQxODNDMzdDNzczQkYzN0M3NzM4M0NERjc1M0VBQ0RGNzUzNTNBQUZENUY1QkFBRkQ1Rjk5NkYzRERGMTQ2RjNEREY2MURCNDQ3ODg2REI0NDc4MTdGM0FGQ0E4MUYzQUZDQTJCQzQ2OEI5M0VDNDY4QjkwNDM0MjQzODJDMzQyNDM4N0U0MEEzQzI1RjQwQTNDMkJBQzMxRDE2NzJDMzFEMTY3NzI1RTJCQzBDMjVFMkJDRDY0OTNDMjg4QjQ5M0MyODI2OTUwREZGNDE5NTBERkZFMTAxQTgzOTcxMDFBODM5NjlCMzBDMDhERUIzMEMwODE0RTRCNEQ4OUNFNEI0RDg2M0MxNTY2NDkwQzE1NjY0NTU4NENCN0I2MTg0Q0I3QjIxQjYzMkQ1NzBCNjMyRDUwQzVDNkM0ODc0NUM2QzQ4N0Q1N0I4RDA0MjU3QjhEMA=="
            .Base64DecodeUtf8().ToBytes();
        _hashTableE1 = "NTBDNEUzM0FFNkZFNEQ4QUREM0ZFNjlCNzI4ODY2Qzc3QTI1Rjc5NUYwNjgwOTczNkI4RTM2QUVBQ0U4QkVEQ0ZDQjQ2QzM5MENEQzY1NEE2NzUyNTNFNENCQkFFRDM4RkJBQjk4NjhGNzc3RkQyMjkwMjVBRUM2NUI5RjQzRkU0MDkyNDM3QUI3RTVCRTU4MjdDMDEwOUU3QzVGNTM2MDkwODI4Qzg3Mjc2NzMyREYwMEE3MjI0MTdDRjg3MTIxNkQ5MkNEMDQ0QUY1RkZEQjRBNTJERDlBMzZBQUFDQkI4Nzk3NjFENUNENjI5RTBFODczMDQzOTRCMTlBRUYyRjkyNUZEOThBNUYzRDQ3ODREODBEMDQxMDY5OTdFQjNGRTdBNjUxNzhCODlCMTZGQzYwOTYxMkVDMDkwMUY5RDNENzJEQTc4MTJCM0IzQzM5QzcyOUFBNTkxNEQwQUI1MA=="
            .Base64DecodeUtf8().ToBytes();
        _hashTableD1 = "RDcyREE3ODEyQjNCM0MzOUM3MjlBQTU5MTREMEFCNTAyOTc0NEY3QTY1ODE0MjZGRjhEQTlBQjA2MjVERDZDQjk3QTVGQjU3NENGNTBEMTU5RDVCRDhERjlBODc0QzdCNjU1N0QwNDZEQjUwRjY0MkQxQUVENUNBMDdEQzk0QTQ5NDg0NTg3RUJFMDcyNjA0MEFGRTIzODhENjcyNDE2RTFGNjMzMTU0MkE4MzdFN0FCNEY5MDU4Q0RDOEM2MkU2Q0E1MjA0NzczNUUwNEYyRTlFN0E3QkY2Njg3NTY3NkE4NkZEQkU2NUZGQjI0QjU5QUI5QTM0RDhGNjBGMUM5Q0VFRDM0NDY0Nzk0RkY1M0M1NDI4N0Y4MTVEOTUyODQ0NzNBMzg0Njk5NzlDQjE1ODJENjc4QUJEMDlCRDU3QzU1MEM0RTMzQUU2RkU0RDhBREQzRkU2OUI3Mjg4NjZDNw=="
            .Base64DecodeUtf8().ToBytes();

        _hashTableC = "RjM1RDBDNjVEQkU3NDBBMjk5MTNBQjA5MDM3RjQ3NUY4Mjg0Q0FFOTYxNTBGNEM2QzM2REIxQjk1MzU0QUNDNEZGQUI3N0FDNDdCRTkxOUY5NUY2Rjg1OTY1NEVBNTVBNEYyMkNCRjQwOTRBMEE5OUVDRTMxREZFRTc2NzE2QjIyNDcwMDRFMDUxODIxQjVEODVFMjk2NTdDRTg2RDU5NDkzQkYzQUMwQjFCNUE0Qzg0NEJDQ0NENzZCN0UyRTg4N0QyQjAzMTc0OUU5NjkwMEI0NUMxOUVBMjBGMkNBRUU5NzlDNkMzMjM0QTg4RjlENzEwNkU4M0IwQzdDQkRCMDcyRjM4QjYyMjU3QjFDQkFEMTI2RjcwNTdGM0Y5MjgwOEU1RjUwNENDNjU4RTFCNkExNzVERUM3RjE4MUNEMjNEQkRDMjdERDU1NkFEQUZDNDJBNkE3QTMwMUIzRDBFRDc2RkE0QkI3MERCODc0RTY2RUEwOEQzODczRDMyRjMwNzg4NzJENTI2RjM5ODQzRDQwNTQyMTBCMTUzMzU2RTQ0MUMxRjBDMjQ2RkJDRkVGRDlDNDdBQjlENjYwMTE2NjM2OEExQUJCOUEyQUZEMzdDNTE0OTgzNUEyMTA4QzZEQzMwMkM5Rjk4MzQzQURBRUFGMDcwRTI5MEY5MDUzRDg5QjEzM0VENDRENjM1QkVCMUVGNTFGOUVFNTQ1MzExMjY0NjgyQ0E5M0MwOERGNzlEMjI4ODk1RUFBNjE0ODE4"
            .Base64DecodeUtf8().ToBytes();
    }

    public void Encrypt(ref Span<byte> data)
    {
        // Allocate stack
        Span<uint> localContainerA = stackalloc uint[ContainerLength];
        Span<uint> localContainerB = stackalloc uint[ContainerLength];
        Span<uint> localContainerC = stackalloc uint[ContainerLength];
        // Create pointers
        var hashTableE0SpanByte1 = new ReadOnlySpan<byte>(_hashTableE0, 1, _hashTableE0.Length - 1);
        var hashTableE0SpanUint1 = MemoryMarshal.Cast<byte, uint>(hashTableE0SpanByte1);
        var hashTableE0SpanByte2 = new ReadOnlySpan<byte>(_hashTableE0, 2, _hashTableE0.Length - 2);
        var hashTableE0SpanUint2 = MemoryMarshal.Cast<byte, uint>(hashTableE0SpanByte2);
        var hashTableE0SpanByte3 = new ReadOnlySpan<byte>(_hashTableE0, 3, _hashTableE0.Length - 3);
        var hashTableE0SpanUint3 = MemoryMarshal.Cast<byte, uint>(hashTableE0SpanByte3);
        var hashTableE0SpanByte4 = new ReadOnlySpan<byte>(_hashTableE0, 4, _hashTableE0.Length - 4);
        var hashTableE0SpanUint4 = MemoryMarshal.Cast<byte, uint>(hashTableE0SpanByte4);
        var hashTableE1SpanByte0 = new ReadOnlySpan<byte>(_hashTableE1, 0, 32);
        var hashTableE1SpanUint0 = MemoryMarshal.Cast<byte, uint>(hashTableE1SpanByte0);
        var hashTableE1SpanByte20 = new ReadOnlySpan<byte>(_hashTableE1, 32, 128);
        var hashTableE1SpanUint20 = MemoryMarshal.Cast<byte, uint>(hashTableE1SpanByte20);
        var hashTableE1SpanByte100 = new ReadOnlySpan<byte>(_hashTableE1, 160, 16);
        var hashTableE1SpanUint100 = MemoryMarshal.Cast<byte, uint>(hashTableE1SpanByte100);
        var dataSpanUint = MemoryMarshal.Cast<byte, uint>(data);
        var localContainerASpanByte = MemoryMarshal.Cast<uint, byte>(localContainerA);

        var laps = data.Length / ContainerLengthBytes;
        _privateKey.CopyTo(localContainerC);
        for (var z = 0; z < laps; z++)
        {
            for (var i = 0; i < ContainerLength; i++)
                localContainerA[i] = dataSpanUint[z * ContainerLength + i] ^ localContainerC[i] ^ hashTableE1SpanUint0[i];

            for (var i = 0; i < ContainerLength; i++)
                localContainerB[i] = hashTableE1SpanUint0[^(4 - i)] ^ hashTableE0SpanUint1[2 * (byte)(localContainerA[_sequence[i]] >> 8)] ^ hashTableE0SpanUint2[2 * (byte)((localContainerA[_sequence[i + 1]] & 0xFF0000) >> 16)] ^ hashTableE0SpanUint3[2 * (byte)((localContainerA[_sequence[i + 2]] & 0xFF000000) >> 24)] ^ hashTableE0SpanUint4[2 * (byte)localContainerA[_sequence[i + 3]]];

            for (var i = 0; i < ContainerLength; i++)
            {
                for (var j = 0; j < ContainerLength; j++)
                    localContainerA[j] = hashTableE1SpanUint20[i * 8 + _sequence[j]] ^ hashTableE0SpanUint1[2 * (byte)((localContainerB[_sequence[j + 1]] & 0xFF0000) >> 16)] ^ hashTableE0SpanUint2[2 * (byte)(localContainerB[_sequence[j + 2]] >> 8)] ^ hashTableE0SpanUint3[2 * (byte)localContainerB[_sequence[j + 3]]] ^ hashTableE0SpanUint4[((byte)((localContainerB[_sequence[j]] & 0xFF000000) >> 24) << 3) / 4];

                for (var j = 0; j < ContainerLength; j++)
                    localContainerB[j] = hashTableE1SpanUint20[i * 8 + _sequence[j + 3] + 4] ^ hashTableE0SpanUint1[2 * (byte)((localContainerA[_sequence[j + 3]] & 0xFF0000) >> 16)] ^ hashTableE0SpanUint2[2 * (byte)(localContainerA[_sequence[j]] >> 8)] ^ hashTableE0SpanUint3[2 * (byte)localContainerA[_sequence[j + 1]]] ^ hashTableE0SpanUint4[((byte)((localContainerA[_sequence[j + 2]] & 0xFF000000) >> 24) << 3) / 4];
            }

            for (var i = 0; i < ContainerLength; i++)
            {
                localContainerASpanByte[i * ContainerLength + 0] = hashTableE0SpanByte1[(int)(8 * ((localContainerB[_sequence[i + 3]] & 0xFF000000) >> 24))];
                localContainerASpanByte[i * ContainerLength + 1] = hashTableE0SpanByte1[(int)(8 * ((localContainerB[_sequence[i]] & 0xFF0000) >> 16))];
                localContainerASpanByte[i * ContainerLength + 2] = hashTableE0SpanByte1[8 * (byte)(localContainerB[_sequence[i + 1]] >> 8)];
                localContainerASpanByte[i * ContainerLength + 3] = hashTableE0SpanByte1[8 * (byte)localContainerB[_sequence[i + 2]]];
            }

            for (var i = 0; i < ContainerLength; i++)
            {
                localContainerC[i] = localContainerA[i] ^ hashTableE1SpanUint100[i];
                dataSpanUint[z * ContainerLength + i] = localContainerC[i];
            }
        }
    }
    
    public void Decrypt(ref Span<byte> data)
    {
        // Allocate stack
        Span<uint> localContainerA = stackalloc uint[ContainerLength];
        Span<uint> localContainerB = stackalloc uint[ContainerLength];
        Span<uint> localContainerC = stackalloc uint[ContainerLength];
        // Create pointers
        var hashTableD0SpanByte0 = new ReadOnlySpan<byte>(_hashTableD0);
        var hashTableD0SpanByte1 = new ReadOnlySpan<byte>(_hashTableD0, 1, _hashTableD0.Length - 1);
        var hashTableD0SpanUint1 = MemoryMarshal.Cast<byte, uint>(hashTableD0SpanByte1);
        var hashTableD0SpanByte2 = new ReadOnlySpan<byte>(_hashTableD0, 2, _hashTableD0.Length - 2);
        var hashTableD0SpanUint2 = MemoryMarshal.Cast<byte, uint>(hashTableD0SpanByte2);
        var hashTableD0SpanByte3 = new ReadOnlySpan<byte>(_hashTableD0, 3, _hashTableD0.Length - 3);
        var hashTableD0SpanUint3 = MemoryMarshal.Cast<byte, uint>(hashTableD0SpanByte3);
        var hashTableD0SpanByte4 = new ReadOnlySpan<byte>(_hashTableD0, 4, _hashTableD0.Length - 4);
        var hashTableD0SpanUint4 = MemoryMarshal.Cast<byte, uint>(hashTableD0SpanByte4);
        var hashTableD1SpanByte0 = new ReadOnlySpan<byte>(_hashTableD1, 0, 32);
        var hashTableD1SpanUint0 = MemoryMarshal.Cast<byte, uint>(hashTableD1SpanByte0);
        var hashTableD1SpanByte20 = new ReadOnlySpan<byte>(_hashTableD1, 32, 128);
        var hashTableD1SpanUint20 = MemoryMarshal.Cast<byte, uint>(hashTableD1SpanByte20);
        var hashTableD1SpanByte100 = new ReadOnlySpan<byte>(_hashTableD1, 160, 16);
        var hashTableD1SpanUint100 = MemoryMarshal.Cast<byte, uint>(hashTableD1SpanByte100);
        var dataSpanUint = MemoryMarshal.Cast<byte, uint>(data);
        var localContainerASpanByte = MemoryMarshal.Cast<uint, byte>(localContainerA);

        var laps = data.Length / ContainerLengthBytes;
        _privateKey.CopyTo(localContainerC);
        for (var z = 0; z < laps; z++)
        {
            for (var i = 0; i < ContainerLength; i++)
                localContainerA[i] = dataSpanUint[z * ContainerLength + i] ^ hashTableD1SpanUint0[i];

            for (var i = 0; i < ContainerLength; i++)
                localContainerB[i] = hashTableD1SpanUint0[^(4 - i)] ^ hashTableD0SpanUint1[2 * (byte)(localContainerA[_sequence[i + 2]] >> 8)] ^ hashTableD0SpanUint2[2 * (byte)((localContainerA[_sequence[i + 1]] & 0xFF0000) >> 16)] ^ hashTableD0SpanUint3[2 * (byte)((localContainerA[_sequence[i]] & 0xFF000000) >> 24)] ^ hashTableD0SpanUint4[2 * (byte)localContainerA[_sequence[i + 3]]];

            for (var i = 0; i < ContainerLength; i++)
            {
                for (var j = 0; j < ContainerLength; j++)
                    localContainerA[j] = hashTableD1SpanUint20[i * 8 + _sequence[j + 3]] ^ hashTableD0SpanUint1[2 * (byte)((localContainerB[_sequence[j + 2]] & 0xFF0000) >> 16)] ^ hashTableD0SpanUint2[2 * (byte)(localContainerB[_sequence[j + 1]] >> 8)] ^ hashTableD0SpanUint3[2 * (byte)localContainerB[_sequence[j]]] ^ hashTableD0SpanUint4[2 * (byte)((localContainerB[_sequence[j + 3]] & 0xFF000000) >> 24)];

                for (var j = 0; j < ContainerLength; j++)
                    localContainerB[j] = hashTableD1SpanUint20[i * 8 + _sequence[j + 3] + 4] ^ hashTableD0SpanUint1[2 * (byte)((localContainerA[_sequence[j + 2]] & 0xFF0000) >> 16)] ^ hashTableD0SpanUint2[2 * (byte)(localContainerA[_sequence[j + 1]] >> 8)] ^ hashTableD0SpanUint3[2 * (byte)localContainerA[_sequence[j]]] ^ hashTableD0SpanUint4[2 * (byte)((localContainerA[_sequence[j + 3]] & 0xFF000000) >> 24)];
            }

            for (var i = 0; i < ContainerLength; i++)
            {
                localContainerASpanByte[i * ContainerLength + 0] = hashTableD0SpanByte0[(int)(8 * ((localContainerB[_sequence[i + 3]] & 0xFF000000) >> 24))];
                localContainerASpanByte[i * ContainerLength + 1] = hashTableD0SpanByte0[(int)(8 * ((localContainerB[_sequence[i + 2]] & 0xFF0000) >> 16))];
                localContainerASpanByte[i * ContainerLength + 2] = hashTableD0SpanByte0[8 * (byte)(localContainerB[_sequence[i + 1]] >> 8)];
                localContainerASpanByte[i * ContainerLength + 3] = hashTableD0SpanByte0[8 * (byte)localContainerB[_sequence[i]]];
            }
            
            for (var i = 0; i < ContainerLength; i++)
            {
                localContainerA[i] ^= hashTableD1SpanUint100[i] ^ localContainerC[i];
                localContainerC[i] = dataSpanUint[z * ContainerLength + i];
                dataSpanUint[z * ContainerLength + i] = localContainerA[i];
            }
        }
    }

    public void CalculateChecksum(ref Span<byte> checksumContainer, ReadOnlySpan<byte> data)
    {
        // Create pointers
        var hashTableC0 = new ReadOnlySpan<byte>(_hashTableC, 0, 32);
        var hashTableC20 = new ReadOnlySpan<byte>(_hashTableC, 32, _hashTableC.Length - 32);

        for (var i = 0; i < data.Length; i++)
        {
            var v1 = data[i] + checksumContainer[0] + (checksumContainer[1] << 8);
            checksumContainer[0] = (byte)v1;
            checksumContainer[1] = (byte)(v1 >> 8);

            var pos = 2 * ((i >> 2) & 1);
            v1 = data[i] + checksumContainer[pos + 2] + (checksumContainer[pos + 3] << 8);
            checksumContainer[pos + 2] = (byte)v1;
            checksumContainer[pos + 3] = (byte)(v1 >> 8);

            pos = 2 * (i & 3) + 6;
            v1 = data[i] + checksumContainer[pos] + (checksumContainer[pos + 1] << 8);
            checksumContainer[pos] = (byte)v1;
            checksumContainer[pos + 1] = (byte)(v1 >> 8);
        }

        for (var i = 0; i < 31; i++)
            checksumContainer[i + 1] += (byte)(2 * i + checksumContainer[i]);

        for (var i = 0; i < 32; i++)
            checksumContainer[i] = (byte)(hashTableC20[checksumContainer[i]] + hashTableC0[i]);
    }
}